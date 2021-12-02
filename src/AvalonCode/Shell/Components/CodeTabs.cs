using AvalonCode.Controls;
using AvalonCode.Services;
using AvalonCode.Services.Models;
using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaReactorUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Shell.Components
{
    //public class DocumentTab
    //{
    //    public DocumentTab(IDocument document)
    //    {
    //        Document = document;
    //    }

    //    public IDocument Document { get; }
    //}

    public class CodeTabs : RxComponent
    {
        private TextEditor? _textEditor;

        public override VisualNode Render()
        {
            var applicationParameters = GetParameter<ApplicationParameters>();
            //var documents = applicationParameters.Value.Documents.Select(_ => new DocumentTab(_)).ToArray();

            return new RxGrid("32 *", "*")
            {
                new RxScrollViewer()
                { 
                    new RxStackPanel()
                    {
                        applicationParameters.Value.Documents.Select(RenderDocumentTitle).ToArray()
                    }
                    .Orientation(Avalonia.Layout.Orientation.Horizontal)
                },
                //new RxTextEditor(r => _textEditor = r)
                //    .Document(LoadTextEditorDocument(applicationParameters.Value.CurrentDocument))
                //    .GridRow(1)
            };
        }

        //private AvaloniaEdit.Document.TextDocument LoadTextEditorDocument(IDocumentItem? currentDocument)
        //{
        //    if (currentDocument == null)
        //    {
        //        throw new InvalidOperationException();
        //    }

        //    currentDocument.Document.

            
        //    return new AvaloniaEdit.Document.TextDocument(currentDocument)
        //}

        private VisualNode RenderDocumentTitle(IDocumentItem document)
        {
            var applicationParameters = GetParameter<ApplicationParameters>();
            return new RxToggleButton()
                .Content(document.Name)
                .IsChecked(applicationParameters.Value.CurrentDocument == document)
                .OnChecked(() => OnDocumentSelected(document));
        }

        private void OnDocumentSelected(IDocumentItem document)
        {
            var applicationParameters = GetParameter<ApplicationParameters>();
            applicationParameters.Set(_ =>
            {
                _.CurrentDocument = document;
            });
        }
    }
}
