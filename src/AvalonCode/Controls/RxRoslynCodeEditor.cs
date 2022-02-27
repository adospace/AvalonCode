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

using AvaloniaReactorUI.Internals;
using AvaloniaReactorUI;
using RoslynPad.Editor;

namespace AvalonCode.Controls
{
    public partial interface IRxRoslynCodeEditor : IRxCodeTextEditor
    {
        PropertyValue<bool>? IsBraceCompletionEnabled { get; set; }
        PropertyValue<IImage>? ContextActionsIcon { get; set; }

        Action? CreatingDocumentAction { get; set; }
        Action<CreatingDocumentEventArgs>? CreatingDocumentActionWithArgs { get; set; }
    }

    public partial class RxRoslynCodeEditor<T> : RxCodeTextEditor<T>, IRxRoslynCodeEditor where T : RoslynCodeEditor, new()
    {
        public RxRoslynCodeEditor()
        {

        }

        public RxRoslynCodeEditor(Action<T?> componentRefAction)
            : base(componentRefAction)
        {

        }

        PropertyValue<bool>? IRxRoslynCodeEditor.IsBraceCompletionEnabled { get; set; }
        PropertyValue<IImage>? IRxRoslynCodeEditor.ContextActionsIcon { get; set; }

        Action? IRxRoslynCodeEditor.CreatingDocumentAction { get; set; }
        Action<CreatingDocumentEventArgs>? IRxRoslynCodeEditor.CreatingDocumentActionWithArgs { get; set; }

        protected override void OnUpdate()
        {
            OnBeginUpdate();

            Validate.EnsureNotNull(NativeControl);
            var thisAsIRxRoslynCodeEditor = (IRxRoslynCodeEditor)this;
            NativeControl.Set(RoslynCodeEditor.IsBraceCompletionEnabledProperty, thisAsIRxRoslynCodeEditor.IsBraceCompletionEnabled);
            NativeControl.Set(RoslynCodeEditor.ContextActionsIconProperty, thisAsIRxRoslynCodeEditor.ContextActionsIcon);


            base.OnUpdate();

            OnEndUpdate();
        }

        partial void OnBeginUpdate();
        partial void OnEndUpdate();

        protected override void OnAttachNativeEvents()
        {
            Validate.EnsureNotNull(NativeControl);

            var thisAsIRxRoslynCodeEditor = (IRxRoslynCodeEditor)this;
            if (thisAsIRxRoslynCodeEditor.CreatingDocumentAction != null || thisAsIRxRoslynCodeEditor.CreatingDocumentActionWithArgs != null)
            {
                NativeControl.CreatingDocument += NativeControl_CreatingDocument;
            }

            base.OnAttachNativeEvents();
        }

        private void NativeControl_CreatingDocument(object? sender, CreatingDocumentEventArgs e)
        {
            var thisAsIRxRoslynCodeEditor = (IRxRoslynCodeEditor)this;
            thisAsIRxRoslynCodeEditor.CreatingDocumentAction?.Invoke();
            thisAsIRxRoslynCodeEditor.CreatingDocumentActionWithArgs?.Invoke(e);
        }

        protected override void OnDetachNativeEvents()
        {
            if (NativeControl != null)
            {
                NativeControl.CreatingDocument -= NativeControl_CreatingDocument;
            }

            base.OnDetachNativeEvents();
        }
    }
    public partial class RxRoslynCodeEditor : RxRoslynCodeEditor<RoslynCodeEditor>
    {
        public RxRoslynCodeEditor()
        {

        }

        public RxRoslynCodeEditor(Action<RoslynCodeEditor?> componentRefAction)
            : base(componentRefAction)
        {

        }
    }
    public static partial class RxRoslynCodeEditorExtensions
    {
        public static T IsBraceCompletionEnabled<T>(this T roslyncodeeditor, bool isBraceCompletionEnabled) where T : IRxRoslynCodeEditor
        {
            roslyncodeeditor.IsBraceCompletionEnabled = new PropertyValue<bool>(isBraceCompletionEnabled);
            return roslyncodeeditor;
        }
        public static T ContextActionsIcon<T>(this T roslyncodeeditor, IImage contextActionsIcon) where T : IRxRoslynCodeEditor
        {
            roslyncodeeditor.ContextActionsIcon = new PropertyValue<IImage>(contextActionsIcon);
            return roslyncodeeditor;
        }
        public static T OnCreatingDocument<T>(this T roslyncodeeditor, Action creatingdocumentAction) where T : IRxRoslynCodeEditor
        {
            roslyncodeeditor.CreatingDocumentAction = creatingdocumentAction;
            return roslyncodeeditor;
        }

        public static T OnCreatingDocument<T>(this T roslyncodeeditor, Action<CreatingDocumentEventArgs> creatingdocumentActionWithArgs) where T : IRxRoslynCodeEditor
        {
            roslyncodeeditor.CreatingDocumentActionWithArgs = creatingdocumentActionWithArgs;
            return roslyncodeeditor;
        }
    }
}
