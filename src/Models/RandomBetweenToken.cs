using System;
using System.Collections.Generic;

namespace Lignator.Models
{
    public class RandomBetweenToken : Token
    {
        public override string Key { get; internal set; } = "randombetween";
        public int Lower { get; set; }
        public int Upper { get; set; }
        public override string Tranform(Random random = null, IDictionary<string, string> variables = null)
        {
            return random.Next(this.Lower, this.Upper + 1).ToString();
        }
    }
}