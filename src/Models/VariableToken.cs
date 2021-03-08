// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Lignator.Models
{
    public class VariableToken : Token
    {
        private readonly string name;

        public VariableToken(string name)
        {
            this.name = name;
        }
        public override string Key { get; internal set; } = "variable";

        public override string Tranform(Random random = null, IDictionary<string, string> variables = null)
        {
            return variables[this.name];
        }
    }
}