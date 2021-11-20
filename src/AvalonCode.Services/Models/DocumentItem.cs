﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services.Models
{
    public class DocumentItem : SolutionItem
    {
        public DocumentItem(Document document) : base(document.Name)
        {
            Document = document;
        }

        public Document Document { get; }
    }
}