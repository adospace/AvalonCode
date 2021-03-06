using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services
{
    public interface IDocumentItem : ISolutionItem
    {
        Guid Id { get; }

        Document Document { get; }

        Task<ILoadedDocumentItem> Load(CancellationToken cancellationToken = default);
    }
}
