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
    public partial interface IRxCodeTextEditor : IRxTextEditor
    {
        PropertyValue<IBrush>? CompletionBackground { get; set; }

        Action? ToolTipRequestAction { get; set; }
        Action<ToolTipRequestEventArgs>? ToolTipRequestActionWithArgs { get; set; }
    }

    public partial class RxCodeTextEditor<T> : RxTextEditor<T>, IRxCodeTextEditor where T : CodeTextEditor, new()
    {
        public RxCodeTextEditor()
        {

        }

        public RxCodeTextEditor(Action<T?> componentRefAction)
            : base(componentRefAction)
        {

        }

        PropertyValue<IBrush>? IRxCodeTextEditor.CompletionBackground { get; set; }

        Action? IRxCodeTextEditor.ToolTipRequestAction { get; set; }
        Action<ToolTipRequestEventArgs>? IRxCodeTextEditor.ToolTipRequestActionWithArgs { get; set; }

        protected override void OnUpdate()
        {
            OnBeginUpdate();

            Validate.EnsureNotNull(NativeControl);
            var thisAsIRxCodeTextEditor = (IRxCodeTextEditor)this;
            NativeControl.Set(CodeTextEditor.CompletionBackgroundProperty, thisAsIRxCodeTextEditor.CompletionBackground);


            base.OnUpdate();

            OnEndUpdate();
        }

        partial void OnBeginUpdate();
        partial void OnEndUpdate();

        protected override void OnAttachNativeEvents()
        {
            Validate.EnsureNotNull(NativeControl);

            var thisAsIRxCodeTextEditor = (IRxCodeTextEditor)this;
            if (thisAsIRxCodeTextEditor.ToolTipRequestAction != null || thisAsIRxCodeTextEditor.ToolTipRequestActionWithArgs != null)
            {
                NativeControl.ToolTipRequest += NativeControl_ToolTipRequest;
            }

            base.OnAttachNativeEvents();
        }

        private void NativeControl_ToolTipRequest(object? sender, ToolTipRequestEventArgs e)
        {
            var thisAsIRxCodeTextEditor = (IRxCodeTextEditor)this;
            thisAsIRxCodeTextEditor.ToolTipRequestAction?.Invoke();
            thisAsIRxCodeTextEditor.ToolTipRequestActionWithArgs?.Invoke(e);
        }

        protected override void OnDetachNativeEvents()
        {
            if (NativeControl != null)
            {
                NativeControl.ToolTipRequest -= NativeControl_ToolTipRequest;
            }

            base.OnDetachNativeEvents();
        }
    }
    public partial class RxCodeTextEditor : RxCodeTextEditor<CodeTextEditor>
    {
        public RxCodeTextEditor()
        {

        }

        public RxCodeTextEditor(Action<CodeTextEditor?> componentRefAction)
            : base(componentRefAction)
        {

        }
    }
    public static partial class RxCodeTextEditorExtensions
    {
        public static T CompletionBackground<T>(this T codetexteditor, IBrush completionBackground) where T : IRxCodeTextEditor
        {
            codetexteditor.CompletionBackground = new PropertyValue<IBrush>(completionBackground);
            return codetexteditor;
        }
        public static T OnToolTipRequest<T>(this T codetexteditor, Action tooltiprequestAction) where T : IRxCodeTextEditor
        {
            codetexteditor.ToolTipRequestAction = tooltiprequestAction;
            return codetexteditor;
        }

        public static T OnToolTipRequest<T>(this T codetexteditor, Action<ToolTipRequestEventArgs> tooltiprequestActionWithArgs) where T : IRxCodeTextEditor
        {
            codetexteditor.ToolTipRequestActionWithArgs = tooltiprequestActionWithArgs;
            return codetexteditor;
        }
    }
}

