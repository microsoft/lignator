// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Lignator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lignator.Interfaces
{
    public interface ITokenExtractor
    {
        Task<List<Extraction>> Extract(string template, bool multiLine = false);
    }
}