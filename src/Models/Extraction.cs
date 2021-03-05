// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
namespace Lignator.Models
{
    public class Extraction
    {
        public string SourceFileName { get; set; }
        public string Template { get; set; }
        public List<Token> Tokens { get; set; }
        public string UnProcessedTemplate { get; set; }
    }
}