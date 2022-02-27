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
using AvalonCode.Controls.Native;
using System.Composition;

namespace AvalonCode.Controls
{
    public partial interface IRxRoslynDocumentEditor : IRxCodeTextEditor
    {
        PropertyValue<bool>? IsBraceCompletionEnabled { get; set; }
        PropertyValue<IImage>? ContextActionsIcon { get; set; }

    }

    public partial class RxRoslynDocumentEditor<T> : RxCodeTextEditor<T>, IRxRoslynDocumentEditor where T : RoslynDocumentEditor, new()
    {
        public RxRoslynDocumentEditor()
        {

        }

        public RxRoslynDocumentEditor(Action<T?> componentRefAction)
            : base(componentRefAction)
        {

        }

        PropertyValue<bool>? IRxRoslynDocumentEditor.IsBraceCompletionEnabled { get; set; }
        PropertyValue<IImage>? IRxRoslynDocumentEditor.ContextActionsIcon { get; set; }


        protected override void OnUpdate()
        {
            OnBeginUpdate();

            Validate.EnsureNotNull(NativeControl);
            var thisAsIRxRoslynDocumentEditor = (IRxRoslynDocumentEditor)this;
            NativeControl.Set(RoslynDocumentEditor.IsBraceCompletionEnabledProperty, thisAsIRxRoslynDocumentEditor.IsBraceCompletionEnabled);
            NativeControl.Set(RoslynDocumentEditor.ContextActionsIconProperty, thisAsIRxRoslynDocumentEditor.ContextActionsIcon);


            base.OnUpdate();

            OnEndUpdate();
        }

        partial void OnBeginUpdate();
        partial void OnEndUpdate();

        protected override void OnAttachNativeEvents()
        {
            Validate.EnsureNotNull(NativeControl);

            var thisAsIRxRoslynDocumentEditor = (IRxRoslynDocumentEditor)this;


            base.OnAttachNativeEvents();
        }


        protected override void OnDetachNativeEvents()
        {

            base.OnDetachNativeEvents();
        }
    }
    public partial class RxRoslynDocumentEditor : RxRoslynDocumentEditor<RoslynDocumentEditor>
    {
        public RxRoslynDocumentEditor()
        {

        }

        public RxRoslynDocumentEditor(Action<RoslynDocumentEditor?> componentRefAction)
            : base(componentRefAction)
        {

        }
    }
    public static partial class RxRoslynDocumentEditorExtensions
    {
        public static T IsBraceCompletionEnabled<T>(this T RoslynDocumentEditor, bool isBraceCompletionEnabled) where T : IRxRoslynDocumentEditor
        {
            RoslynDocumentEditor.IsBraceCompletionEnabled = new PropertyValue<bool>(isBraceCompletionEnabled);
            return RoslynDocumentEditor;
        }
        public static T ContextActionsIcon<T>(this T RoslynDocumentEditor, IImage contextActionsIcon) where T : IRxRoslynDocumentEditor
        {
            RoslynDocumentEditor.ContextActionsIcon = new PropertyValue<IImage>(contextActionsIcon);
            return RoslynDocumentEditor;
        }

    }
}
