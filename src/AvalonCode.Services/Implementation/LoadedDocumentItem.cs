using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services.Implementation
{
    internal class LoadedDocumentItem : ILoadedDocumentItem
    {
        private string _sourceCode;

        public LoadedDocumentItem(IDocumentItem documentItem, string sourceCode)
        {
            DocumentItem = documentItem;
            _sourceCode = sourceCode;
        }

        public IDocumentItem DocumentItem { get; }

        public string SourceCode
        {
            get => _sourceCode; set
            {
                _sourceCode = value;
                IsDirty = true;
            }
        }

        public bool IsDirty { get; private set; }

        public DateTime? LastActivation { get; set; }

        public async Task Save(CancellationToken cancellationToken = default)
        {
            var filePath = DocumentItem.Document.FilePath ?? throw new InvalidOperationException();
            
            await File.WriteAllTextAsync(filePath, _sourceCode, cancellationToken);

            IsDirty = false;
        }
    }
}
