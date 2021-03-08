// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Lignator.Models
{
    public class RandomItemToken : Token
    {
        public override string Key { get; internal set; } = "randomitem";
        public string[] Items  { get; set; }
        public override string Tranform(Random random, IDictionary<string, string> variables = null)
        {
            int index = random.Next(0, this.Items.Length);
            return this.Items[index];
        }
    }
}