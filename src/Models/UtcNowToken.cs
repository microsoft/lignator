using System;
using System.Collections.Generic;

namespace Lignator.Models
{
    public class UtcNowToken : Token
    {
        private string format;
        public UtcNowToken(string format = "yyyy-MM-dd HH:mm:ss.fff")
        {
            this.format = format;
        }
        public override string Key { get; internal set; } = "utcnow";
        public override string Tranform(Random random = null, IDictionary<string, string> variables = null)
        {
            return DateTime.UtcNow.ToString(this.format);
        }
    }
}