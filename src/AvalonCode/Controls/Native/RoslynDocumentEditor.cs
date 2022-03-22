using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynPad.Roslyn;
using RoslynPad.Roslyn.BraceMatching;
using RoslynPad.Roslyn.Diagnostics;
using RoslynPad.Roslyn.QuickInfo;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Input;
using ImageSource = Avalonia.Media.IImage;
using ModifierKeys = Avalonia.Input.KeyModifiers;
using RoutingStrategy = Avalonia.Interactivity.RoutingStrategies;
using RoslynPad.Editor;
using AvalonCode.Services;

namespace AvalonCode.Controls.Native
{
    public class RoslynDocumentEditor : CodeTextEditor, IStyleable
    {
        Type IStyleable.StyleKey => typeof(TextEditor);

        private readonly TextMarkerService _textMarkerService;
        private BraceMatcherHighlightRenderer? _braceMatcherHighlighter;
        private ContextActionsRenderer? _contextActionsRenderer;
        private IClassificationHighlightColors _classificationHighlightColors = new ClassificationHighlightColors();
        private IRoslynHost? _roslynHost;
        private DocumentId? _documentId;
        private IQuickInfoProvider? _quickInfoProvider;
        private IBraceMatchingService? _braceMatchingService;
        private CancellationTokenSource? _braceMatchingCts;
        private RoslynHighlightingColorizer? _colorizer;

        public RoslynDocumentEditor()
        {
            _textMarkerService = new TextMarkerService(this);
            TextArea.TextView.BackgroundRenderers.Add(_textMarkerService);
            TextArea.TextView.LineTransformers.Add(_textMarkerService);
            TextArea.Caret.PositionChanged += CaretOnPositionChanged;
        }

        public bool IsBraceCompletionEnabled
        {
            get { return this.GetValue(IsBraceCompletionEnabledProperty); }
            set { this.SetValue(IsBraceCompletionEnabledProperty, value); }
        }

        public static readonly StyledProperty<bool> IsBraceCompletionEnabledProperty =
            CommonProperty.Register<RoslynDocumentEditor, bool>(nameof(IsBraceCompletionEnabled), defaultValue: true);

        public static readonly StyledProperty<ImageSource> ContextActionsIconProperty = CommonProperty.Register<RoslynDocumentEditor, ImageSource>(
            nameof(ContextActionsIcon), onChanged: OnContextActionsIconChanged);

        private static void OnContextActionsIconChanged(RoslynDocumentEditor editor, CommonPropertyChangedArgs<ImageSource> args)
        {
            if (editor._contextActionsRenderer != null)
            {
                editor._contextActionsRenderer.IconImage = args.NewValue;
            }
        }

        public ImageSource ContextActionsIcon
        {
            get => this.GetValue(ContextActionsIconProperty);
            set => this.SetValue(ContextActionsIconProperty, value);
        }

        public static readonly DirectProperty<RoslynDocumentEditor, ILoadedDocumentItem?> LoadedDocumentProperty =
            AvaloniaProperty.RegisterDirect<RoslynDocumentEditor, ILoadedDocumentItem?>(
                nameof(LoadedDocument),
                o => o.LoadedDocument);

        private ILoadedDocumentItem? _loadedDocument;

        public ILoadedDocumentItem? LoadedDocument
        {
            get { return _loadedDocument; }
        }

        public void LoadDocument(ILoadedDocumentItem loadedDocumentItem)
        {
            _roslynHost = loadedDocumentItem.DocumentItem.GetParent<IRoslynHost>() ?? throw new InvalidOperationException();

            _braceMatcherHighlighter ??= new BraceMatcherHighlightRenderer(TextArea.TextView, _classificationHighlightColors);

            _quickInfoProvider = _roslynHost.GetService<IQuickInfoProvider>();
            _braceMatchingService = _roslynHost.GetService<IBraceMatchingService>();

            //var avalonEditTextContainer = new AvalonEditTextContainer(Document) { Editor = this };

            _documentId = loadedDocumentItem.DocumentItem.Document.Id;

            Document.Remove(0, Document.TextLength);
            AppendText(loadedDocumentItem.SourceCode);
            Document.UndoStack.ClearAll();
            AsyncToolTipRequest = OnAsyncToolTipRequest;

            RefreshHighlighting();

            _contextActionsRenderer = new ContextActionsRenderer(this, _textMarkerService) { IconImage = ContextActionsIcon };
            _contextActionsRenderer.Providers.Add(new RoslynContextActionProvider(_documentId, _roslynHost));

            var completionProvider = new RoslynCodeEditorCompletionProvider(_documentId, _roslynHost);
            completionProvider.Warmup();

            CompletionProvider = completionProvider;

            _loadedDocument = loadedDocumentItem;
        }

        public void RefreshHighlighting()
        {
            if (_colorizer != null)
            {
                TextArea.TextView.LineTransformers.Remove(_colorizer);
            }

            if (_documentId != null && _roslynHost != null && _classificationHighlightColors != null)
            {
                _colorizer = new RoslynHighlightingColorizer(_documentId, _roslynHost, _classificationHighlightColors);
                TextArea.TextView.LineTransformers.Insert(0, _colorizer);
            }
        }

        private async void CaretOnPositionChanged(object? sender, EventArgs eventArgs)
        {
            if (_roslynHost == null || _documentId == null || _braceMatcherHighlighter == null)
            {
                return;
            }

            _braceMatchingCts?.Cancel();

            if (_braceMatchingService == null)
            {
                return;
            }

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            _braceMatchingCts = cts;

            var document = _roslynHost.GetDocument(_documentId);
            if (document == null)
            {
                return;
            }

            try
            {
                var text = await document.GetTextAsync(token).ConfigureAwait(false);
                var caretOffset = CaretOffset;
                if (caretOffset <= text.Length)
                {
                    var result = await _braceMatchingService.GetAllMatchingBracesAsync(document, caretOffset, token).ConfigureAwait(true);
                    _braceMatcherHighlighter.SetHighlight(result.leftOfPosition, result.rightOfPosition);
                }
            }
            catch (OperationCanceledException)
            {
                // Caret moved again, we do nothing because execution stopped before propagating stale data
                // while fresh data is being applied in a different `CaretOnPositionChanged` handler which runs in parallel.
            }
        }

        private void TryJumpToBrace()
        {
            if (_braceMatcherHighlighter == null) return;

            var caret = CaretOffset;

            if (TryJumpToPosition(_braceMatcherHighlighter.LeftOfPosition, caret) ||
                TryJumpToPosition(_braceMatcherHighlighter.RightOfPosition, caret))
            {
                ScrollToLine(TextArea.Caret.Line);
            }
        }

        private bool TryJumpToPosition(BraceMatchingResult? position, int caret)
        {
            if (position != null)
            {
                if (position.Value.LeftSpan.Contains(caret))
                {
                    CaretOffset = position.Value.RightSpan.End;
                    return true;
                }

                if (position.Value.RightSpan.Contains(caret) || position.Value.RightSpan.End == caret)
                {
                    CaretOffset = position.Value.LeftSpan.Start;
                    return true;
                }
            }

            return false;
        }

        private async Task OnAsyncToolTipRequest(ToolTipRequestEventArgs arg)
        {
            if (_roslynHost == null || _documentId == null || _quickInfoProvider == null)
            {
                return;
            }

            // TODO: consider invoking this with a delay, then showing the tool-tip without one
            var document = _roslynHost.GetDocument(_documentId);
            if (document == null)
            {
                return;
            }

            var info = await _quickInfoProvider.GetItemAsync(document, arg.Position, CancellationToken.None).ConfigureAwait(true);
            if (info != null)
            {
                arg.SetToolTip(info.Create());
            }
        }

        protected void ProcessDiagnostics(DiagnosticsUpdatedArgs args)
        {
            if (this.GetDispatcher().CheckAccess())
            {
                ProcessDiagnosticsOnUiThread(args);
                return;
            }

            this.GetDispatcher().InvokeAsync(() => ProcessDiagnosticsOnUiThread(args));
        }

        private void ProcessDiagnosticsOnUiThread(DiagnosticsUpdatedArgs args)
        {
            _textMarkerService.RemoveAll(marker => Equals(args.Id, marker.Tag));

            if (args.Kind != DiagnosticsUpdatedKind.DiagnosticsCreated)
            {
                return;
            }

            foreach (var diagnosticData in args.Diagnostics)
            {
                if (diagnosticData.Severity == DiagnosticSeverity.Hidden || diagnosticData.IsSuppressed)
                {
                    continue;
                }

                var span = diagnosticData.GetTextSpan();
                if (span == null)
                {
                    continue;
                }

                var marker = _textMarkerService.TryCreate(span.Value.Start, span.Value.Length);
                if (marker != null)
                {
                    marker.Tag = args.Id;
                    marker.MarkerColor = GetDiagnosticsColor(diagnosticData);
                    marker.ToolTip = diagnosticData.Message;
                }
            }
        }

        private static Color GetDiagnosticsColor(DiagnosticData diagnosticData)
        {
            return diagnosticData.Severity switch
            {
                DiagnosticSeverity.Info => Colors.LimeGreen,
                DiagnosticSeverity.Warning => Colors.DodgerBlue,
                DiagnosticSeverity.Error => Colors.Red,
                _ => throw new ArgumentOutOfRangeException(nameof(diagnosticData)),
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.HasModifiers(ModifierKeys.Control))
            {
                switch (e.Key)
                {
                    case Key.OemCloseBrackets:
                        TryJumpToBrace();
                        break;
                }
            }
        }
    }
}
