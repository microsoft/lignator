// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.Logging;

namespace Lignator.Interfaces
{
    public interface IFileSink: IDisposable
    {
        IFileSink Start(string path, bool multiline = false, bool clean = false);

        void Sink(string content);
    }
}