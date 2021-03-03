using System;
using System.Collections.Generic;

namespace Lignator.Models
{
    public class LineFromFileToken : Token
    {
        public override string Key { get; internal set; } = "linefromfile";
        public string[] Items { get; set; }
        public override string Tranform(Random random = null, IDictionary<string, string> variables = null)
        {
            int index = random.Next(0, this.Items.Length);
            return this.Items[index];
        }
    }
}