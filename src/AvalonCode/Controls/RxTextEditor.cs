using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections;

using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Platform;
using Avalonia.Controls.Selection;
using Avalonia.Input.TextInput;

using AvaloniaReactorUI;
using AvaloniaReactorUI.Internals;
using System.Text;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;

namespace AvalonCode.Controls
{
    public partial interface IRxTextEditor : IRxTemplatedControl
    {
        PropertyValue<TextDocument>? Document { get; set; }
        PropertyValue<TextEditorOptions>? Options { get; set; }
        PropertyValue<IHighlightingDefinition>? SyntaxHighlighting { get; set; }
        PropertyValue<bool>? WordWrap { get; set; }
        PropertyValue<bool>? IsReadOnly { get; set; }
        PropertyValue<bool>? IsModified { get; set; }
        PropertyValue<bool>? ShowLineNumbers { get; set; }
        PropertyValue<IBrush>? LineNumbersForeground { get; set; }
        PropertyValue<Encoding>? Encoding { get; set; }
        PropertyValue<ScrollBarVisibility>? HorizontalScrollBarVisibility { get; set; }
        PropertyValue<ScrollBarVisibility>? VerticalScrollBarVisibility { get; set; }

        Action? PreviewPointerHoverAction { get; set; }
        Action<PointerEventArgs>? PreviewPointerHoverActionWithArgs { get; set; }
        Action? PointerHoverAction { get; set; }
        Action<PointerEventArgs>? PointerHoverActionWithArgs { get; set; }
        Action? PreviewPointerHoverStoppedAction { get; set; }
        Action<PointerEventArgs>? PreviewPointerHoverStoppedActionWithArgs { get; set; }
        Action? PointerHoverStoppedAction { get; set; }
        Action<PointerEventArgs>? PointerHoverStoppedActionWithArgs { get; set; }
    }

    public partial class RxTextEditor<T> : RxTemplatedControl<T>, IRxTextEditor where T : TextEditor, new()
    {
        public RxTextEditor()
        {

        }

        public RxTextEditor(Action<T?> componentRefAction)
            : base(componentRefAction)
        {

        }

        PropertyValue<TextDocument>? IRxTextEditor.Document { get; set; }
        PropertyValue<TextEditorOptions>? IRxTextEditor.Options { get; set; }
        PropertyValue<IHighlightingDefinition>? IRxTextEditor.SyntaxHighlighting { get; set; }
        PropertyValue<bool>? IRxTextEditor.WordWrap { get; set; }
        PropertyValue<bool>? IRxTextEditor.IsReadOnly { get; set; }
        PropertyValue<bool>? IRxTextEditor.IsModified { get; set; }
        PropertyValue<bool>? IRxTextEditor.ShowLineNumbers { get; set; }
        PropertyValue<IBrush>? IRxTextEditor.LineNumbersForeground { get; set; }
        PropertyValue<Encoding>? IRxTextEditor.Encoding { get; set; }
        PropertyValue<ScrollBarVisibility>? IRxTextEditor.HorizontalScrollBarVisibility { get; set; }
        PropertyValue<ScrollBarVisibility>? IRxTextEditor.VerticalScrollBarVisibility { get; set; }

        Action? IRxTextEditor.PreviewPointerHoverAction { get; set; }
        Action<PointerEventArgs>? IRxTextEditor.PreviewPointerHoverActionWithArgs { get; set; }
        Action? IRxTextEditor.PointerHoverAction { get; set; }
        Action<PointerEventArgs>? IRxTextEditor.PointerHoverActionWithArgs { get; set; }
        Action? IRxTextEditor.PreviewPointerHoverStoppedAction { get; set; }
        Action<PointerEventArgs>? IRxTextEditor.PreviewPointerHoverStoppedActionWithArgs { get; set; }
        Action? IRxTextEditor.PointerHoverStoppedAction { get; set; }
        Action<PointerEventArgs>? IRxTextEditor.PointerHoverStoppedActionWithArgs { get; set; }

        protected override void OnUpdate()
        {
            OnBeginUpdate();

            Validate.EnsureNotNull(NativeControl);
            var thisAsIRxTextEditor = (IRxTextEditor)this;
            NativeControl.Set(TextEditor.DocumentProperty, thisAsIRxTextEditor.Document);
            //NativeControl.Set(TextEditor.OptionsProperty, thisAsIRxTextEditor.Options);
            NativeControl.Set(TextEditor.SyntaxHighlightingProperty, thisAsIRxTextEditor.SyntaxHighlighting);
            NativeControl.Set(TextEditor.WordWrapProperty, thisAsIRxTextEditor.WordWrap);
            NativeControl.Set(TextEditor.IsReadOnlyProperty, thisAsIRxTextEditor.IsReadOnly);
            NativeControl.Set(TextEditor.IsModifiedProperty, thisAsIRxTextEditor.IsModified);
            NativeControl.Set(TextEditor.ShowLineNumbersProperty, thisAsIRxTextEditor.ShowLineNumbers);
            NativeControl.Set(TextEditor.LineNumbersForegroundProperty, thisAsIRxTextEditor.LineNumbersForeground);
            NativeControl.Set(TextEditor.EncodingProperty, thisAsIRxTextEditor.Encoding);
            NativeControl.Set(TextEditor.HorizontalScrollBarVisibilityProperty, thisAsIRxTextEditor.HorizontalScrollBarVisibility);
            NativeControl.Set(TextEditor.VerticalScrollBarVisibilityProperty, thisAsIRxTextEditor.VerticalScrollBarVisibility);


            base.OnUpdate();

            OnEndUpdate();
        }

        partial void OnBeginUpdate();
        partial void OnEndUpdate();

        protected override void OnAttachNativeEvents()
        {
            Validate.EnsureNotNull(NativeControl);

            var thisAsIRxTextEditor = (IRxTextEditor)this;
            if (thisAsIRxTextEditor.PreviewPointerHoverAction != null || thisAsIRxTextEditor.PreviewPointerHoverActionWithArgs != null)
            {
                NativeControl.PreviewPointerHover += NativeControl_PreviewPointerHover;
            }
            if (thisAsIRxTextEditor.PointerHoverAction != null || thisAsIRxTextEditor.PointerHoverActionWithArgs != null)
            {
                NativeControl.PointerHover += NativeControl_PointerHover;
            }
            if (thisAsIRxTextEditor.PreviewPointerHoverStoppedAction != null || thisAsIRxTextEditor.PreviewPointerHoverStoppedActionWithArgs != null)
            {
                NativeControl.PreviewPointerHoverStopped += NativeControl_PreviewPointerHoverStopped;
            }
            if (thisAsIRxTextEditor.PointerHoverStoppedAction != null || thisAsIRxTextEditor.PointerHoverStoppedActionWithArgs != null)
            {
                NativeControl.PointerHoverStopped += NativeControl_PointerHoverStopped;
            }

            base.OnAttachNativeEvents();
        }

        private void NativeControl_PreviewPointerHover(object? sender, PointerEventArgs e)
        {
            var thisAsIRxTextEditor = (IRxTextEditor)this;
            thisAsIRxTextEditor.PreviewPointerHoverAction?.Invoke();
            thisAsIRxTextEditor.PreviewPointerHoverActionWithArgs?.Invoke(e);
        }
        private void NativeControl_PointerHover(object? sender, PointerEventArgs e)
        {
            var thisAsIRxTextEditor = (IRxTextEditor)this;
            thisAsIRxTextEditor.PointerHoverAction?.Invoke();
            thisAsIRxTextEditor.PointerHoverActionWithArgs?.Invoke(e);
        }
        private void NativeControl_PreviewPointerHoverStopped(object? sender, PointerEventArgs e)
        {
            var thisAsIRxTextEditor = (IRxTextEditor)this;
            thisAsIRxTextEditor.PreviewPointerHoverStoppedAction?.Invoke();
            thisAsIRxTextEditor.PreviewPointerHoverStoppedActionWithArgs?.Invoke(e);
        }
        private void NativeControl_PointerHoverStopped(object? sender, PointerEventArgs e)
        {
            var thisAsIRxTextEditor = (IRxTextEditor)this;
            thisAsIRxTextEditor.PointerHoverStoppedAction?.Invoke();
            thisAsIRxTextEditor.PointerHoverStoppedActionWithArgs?.Invoke(e);
        }

        protected override void OnDetachNativeEvents()
        {
            if (NativeControl != null)
            {
                NativeControl.PreviewPointerHover -= NativeControl_PreviewPointerHover;
                NativeControl.PointerHover -= NativeControl_PointerHover;
                NativeControl.PreviewPointerHoverStopped -= NativeControl_PreviewPointerHoverStopped;
                NativeControl.PointerHoverStopped -= NativeControl_PointerHoverStopped;
            }

            base.OnDetachNativeEvents();
        }
    }
    public partial class RxTextEditor : RxTextEditor<TextEditor>
    {
        public RxTextEditor()
        {

        }

        public RxTextEditor(Action<TextEditor?> componentRefAction)
            : base(componentRefAction)
        {

        }
    }
    public static partial class RxTextEditorExtensions
    {
        public static T Document<T>(this T texteditor, TextDocument document) where T : IRxTextEditor
        {
            texteditor.Document = new PropertyValue<TextDocument>(document);
            return texteditor;
        }
        public static T Options<T>(this T texteditor, TextEditorOptions options) where T : IRxTextEditor
        {
            texteditor.Options = new PropertyValue<TextEditorOptions>(options);
            return texteditor;
        }
        public static T SyntaxHighlighting<T>(this T texteditor, IHighlightingDefinition syntaxHighlighting) where T : IRxTextEditor
        {
            texteditor.SyntaxHighlighting = new PropertyValue<IHighlightingDefinition>(syntaxHighlighting);
            return texteditor;
        }
        public static T WordWrap<T>(this T texteditor, bool wordWrap) where T : IRxTextEditor
        {
            texteditor.WordWrap = new PropertyValue<bool>(wordWrap);
            return texteditor;
        }
        public static T IsReadOnly<T>(this T texteditor, bool isReadOnly) where T : IRxTextEditor
        {
            texteditor.IsReadOnly = new PropertyValue<bool>(isReadOnly);
            return texteditor;
        }
        public static T IsModified<T>(this T texteditor, bool isModified) where T : IRxTextEditor
        {
            texteditor.IsModified = new PropertyValue<bool>(isModified);
            return texteditor;
        }
        public static T ShowLineNumbers<T>(this T texteditor, bool showLineNumbers) where T : IRxTextEditor
        {
            texteditor.ShowLineNumbers = new PropertyValue<bool>(showLineNumbers);
            return texteditor;
        }
        public static T LineNumbersForeground<T>(this T texteditor, IBrush lineNumbersForeground) where T : IRxTextEditor
        {
            texteditor.LineNumbersForeground = new PropertyValue<IBrush>(lineNumbersForeground);
            return texteditor;
        }
        public static T Encoding<T>(this T texteditor, Encoding encoding) where T : IRxTextEditor
        {
            texteditor.Encoding = new PropertyValue<Encoding>(encoding);
            return texteditor;
        }
        public static T HorizontalScrollBarVisibility<T>(this T texteditor, ScrollBarVisibility horizontalScrollBarVisibility) where T : IRxTextEditor
        {
            texteditor.HorizontalScrollBarVisibility = new PropertyValue<ScrollBarVisibility>(horizontalScrollBarVisibility);
            return texteditor;
        }
        public static T VerticalScrollBarVisibility<T>(this T texteditor, ScrollBarVisibility verticalScrollBarVisibility) where T : IRxTextEditor
        {
            texteditor.VerticalScrollBarVisibility = new PropertyValue<ScrollBarVisibility>(verticalScrollBarVisibility);
            return texteditor;
        }
        public static T OnPreviewPointerHover<T>(this T texteditor, Action previewpointerhoverAction) where T : IRxTextEditor
        {
            texteditor.PreviewPointerHoverAction = previewpointerhoverAction;
            return texteditor;
        }

        public static T OnPreviewPointerHover<T>(this T texteditor, Action<PointerEventArgs> previewpointerhoverActionWithArgs) where T : IRxTextEditor
        {
            texteditor.PreviewPointerHoverActionWithArgs = previewpointerhoverActionWithArgs;
            return texteditor;
        }
        public static T OnPointerHover<T>(this T texteditor, Action pointerhoverAction) where T : IRxTextEditor
        {
            texteditor.PointerHoverAction = pointerhoverAction;
            return texteditor;
        }

        public static T OnPointerHover<T>(this T texteditor, Action<PointerEventArgs> pointerhoverActionWithArgs) where T : IRxTextEditor
        {
            texteditor.PointerHoverActionWithArgs = pointerhoverActionWithArgs;
            return texteditor;
        }
        public static T OnPreviewPointerHoverStopped<T>(this T texteditor, Action previewpointerhoverstoppedAction) where T : IRxTextEditor
        {
            texteditor.PreviewPointerHoverStoppedAction = previewpointerhoverstoppedAction;
            return texteditor;
        }

        public static T OnPreviewPointerHoverStopped<T>(this T texteditor, Action<PointerEventArgs> previewpointerhoverstoppedActionWithArgs) where T : IRxTextEditor
        {
            texteditor.PreviewPointerHoverStoppedActionWithArgs = previewpointerhoverstoppedActionWithArgs;
            return texteditor;
        }
        public static T OnPointerHoverStopped<T>(this T texteditor, Action pointerhoverstoppedAction) where T : IRxTextEditor
        {
            texteditor.PointerHoverStoppedAction = pointerhoverstoppedAction;
            return texteditor;
        }

        public static T OnPointerHoverStopped<T>(this T texteditor, Action<PointerEventArgs> pointerhoverstoppedActionWithArgs) where T : IRxTextEditor
        {
            texteditor.PointerHoverStoppedActionWithArgs = pointerhoverstoppedActionWithArgs;
            return texteditor;
        }
    }
}
