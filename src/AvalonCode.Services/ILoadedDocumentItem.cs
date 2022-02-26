using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services
{
    public interface ILoadedDocumentItem
    {
        IDocumentItem DocumentItem { get; }

        string SourceCode { get; set; }

        bool IsDirty { get; }

        Task Save(CancellationToken cancellationToken = default);
        
        DateTime? LastActivation { get; set; }
    }
}
