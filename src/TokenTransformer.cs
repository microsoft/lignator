using System;
using System.Collections.Generic;
using System.Linq;
using Lignator.Interfaces;
using Lignator.Models;
using Microsoft.Extensions.Logging;

namespace Lignator
{
    public class TokenTransformer : ITokenTransformer
    {
        private readonly ILogger<TokenTransformer> logger;

        public TokenTransformer(ILogger<TokenTransformer> logger)
        {
            this.logger = logger;
        }

        public string Transform(string template, IEnumerable<Token> tokens, Random random, IDictionary<string, Extraction> variables)
        {
            this.logger.LogTrace("Transforming template with tokens");

            IDictionary<string, string> transformedVariables = null;
            if(variables != null)
            {
                transformedVariables = new Dictionary<string, string>();

                foreach(var variable in variables)
                {
                    transformedVariables.Add(
                        variable.Key,
                        string.Format(variable.Value.Template, variable.Value.Tokens.Select(x => x.Tranform(random)).ToArray())
                    );
                }
            }

            string[] transformed = tokens.Select(x => x.Tranform(random, transformedVariables)).ToArray();
            return string.Format(template, transformed);
        }
    }
}