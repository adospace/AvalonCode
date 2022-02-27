using AvalonCode.Controls;
using AvalonCode.Controls.Native;
using AvalonCode.Services;
using AvalonCode.Services.Models;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaReactorUI;
using RoslynPad.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Shell.Components
{
    public class CodeTabs : RxComponent
    {
        private RoslynDocumentEditor? _textEditor;

        public override VisualNode Render()
        {
            var applicationParameters = GetParameter<ApplicationParameters>();
            App.Current.Styles.TryGetResource("EditorCodeFont", out var fontFamily);

            return new RxGrid("32 *", "*")
            {
                new RxScrollViewer()
                { 
                    new RxStackPanel()
                    {
                        applicationParameters.Value.LoadedDocuments.Select(RenderDocumentTitle).ToArray()
                    }
                    .Orientation(Avalonia.Layout.Orientation.Horizontal)
                },
                applicationParameters.Value.CurrentLoadedDocument == null ? null :
                new RxRoslynDocumentEditor(r => _textEditor = r)
                    .LoadedDocument(applicationParameters.Value.CurrentLoadedDocument)
                    
                    //.SyntaxHighlighting(HighlightingManager.Instance.GetDefinitionByExtension(
                    //    Path.GetExtension(applicationParameters.Value.CurrentLoadedDocument.DocumentItem.Document.FilePath)))
                    .FontFamily(new FontFamily("avares://AvalonCode/Assets#Cascadia Code"))
                    .FontSize(14)
                    .GridRow(1)
            };
        }

        //private AvaloniaEdit.Document.TextDocument LoadTextEditorDocument(ILoadedDocumentItem currentDocument)
        //{
        //    return new AvaloniaEdit.Document.TextDocument(currentDocument.SourceCode);
        //}

        private VisualNode RenderDocumentTitle(ILoadedDocumentItem document)
        {
            var applicationParameters = GetParameter<ApplicationParameters>();
            return new RxToggleButton()
            {
                new RxStackPanel
                {
                    new RxTextBlock(document.DocumentItem.Name),

                    new RxButton()
                        .Content("x")
                        .Padding(5,0)
                        .Margin(5,0)
                        .Background(Brushes.Transparent)
                        .OnClick(()=>OnRemoveDocument(document))
                }
                .Orientation(Avalonia.Layout.Orientation.Horizontal)
            }
            .IsChecked(applicationParameters.Value.CurrentLoadedDocument == document)
            .OnChecked(() => OnDocumentSelected(document));
        }

        private void OnRemoveDocument(ILoadedDocumentItem document)
        {
            var applicationParameters = GetParameter<ApplicationParameters>();

            applicationParameters.Set(_ =>
            {
                _.LoadedDocuments.Remove(document);
                _.CurrentLoadedDocument = _.LoadedDocuments.OrderByDescending(_=>_.LastActivation).FirstOrDefault();
            });
        }

        private void OnDocumentSelected(ILoadedDocumentItem document)
        {
            var applicationParameters = GetParameter<ApplicationParameters>();

            applicationParameters.Set(_ =>
            {
                _.CurrentLoadedDocument = document;
                _.CurrentLoadedDocument.LastActivation = DateTime.Now;
            });
        }
    }
}
