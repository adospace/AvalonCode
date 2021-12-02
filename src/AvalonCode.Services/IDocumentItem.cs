using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services
{
    public interface IDocumentItem
    {
        string Name { get; }

        Guid Id { get; }

        Document Document { get; }
    }
}
