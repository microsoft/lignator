// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Lignator.Models
{
    public class UuidToken : Token
    {
        public override string Key { get; internal set; } = "uuid";
        public override string Tranform(Random random = null, IDictionary<string, string> variables = null)
        {
            return Guid.NewGuid().ToString();
        }
    }
}