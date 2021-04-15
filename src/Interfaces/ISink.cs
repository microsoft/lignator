// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Lignator.Interfaces
{
    public interface ISink: IDisposable
    {
        ISink Start(string path, bool multiline = false, bool clean = false);

        void Sink(string content);
    }
}