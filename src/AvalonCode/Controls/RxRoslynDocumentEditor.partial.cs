using AvalonCode.Services;
using AvaloniaReactorUI.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Controls
{
    public partial interface IRxRoslynDocumentEditor
    {
        ILoadedDocumentItem? LoadedDocument { get; set; }
    }

    public partial class RxRoslynDocumentEditor<T>
    {
        ILoadedDocumentItem? IRxRoslynDocumentEditor.LoadedDocument { get; set; }

        partial void OnBeginUpdate()
        {
            Validate.EnsureNotNull(NativeControl);
            var thisAsIRxRoslynDocumentEditor = (IRxRoslynDocumentEditor)this;
            if (thisAsIRxRoslynDocumentEditor.LoadedDocument != NativeControl.LoadedDocument &&
                thisAsIRxRoslynDocumentEditor.LoadedDocument != null)
            {
                NativeControl.LoadDocument(thisAsIRxRoslynDocumentEditor.LoadedDocument);
            }
        }
    }

    public static partial class RxRoslynDocumentEditorExtensions
    {
        public static T LoadedDocument<T>(this T RoslynDocumentEditor, ILoadedDocumentItem loadedDocument) where T : IRxRoslynDocumentEditor
        {
            RoslynDocumentEditor.LoadedDocument = loadedDocument;
            return RoslynDocumentEditor;
        }
    }
}
