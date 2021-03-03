using System;
using System.Collections.Generic;

namespace Lignator.Models
{
    public abstract class Token
    {
        public abstract string Key { get; internal set; }
        public abstract string Tranform(Random random = null, IDictionary<string, string> variables = null);
    }
}