// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Lignator.Interfaces;
using Lignator.Models;
using Microsoft.Extensions.Logging;

namespace Lignator
{
    public class TokenMapper : ITokenMapper
    {
        private readonly ILogger<TokenMapper> logger;

        public TokenMapper(ILogger<TokenMapper> logger)
        {
            this.logger = logger;
        }
        public async Task<Token> MapToTokenAsync(string rawToken)
        {
            if(rawToken == "uuid")
            {
                this.logger.LogTrace("Mapping uuid token");
                return new UuidToken();
            }

            if(rawToken.StartsWith("utcnow"))
            {
                this.logger.LogTrace("Mapping utcnow token");

                int start = rawToken.IndexOf("(") + 1;
                int end = rawToken.LastIndexOf(")");
                string format = rawToken.Substring(start, end - start);

                return !string.IsNullOrWhiteSpace(format) ? new UtcNowToken(format) : new UtcNowToken();
            }

            if (rawToken.StartsWith("randomitem"))
            {
                this.logger.LogTrace("Mapping RandomItem token");

                int start = rawToken.IndexOf("(") + 1;
                int end = rawToken.IndexOf(")");

                return new RandomItemToken {
                    Items = rawToken.Substring(start, end - start).Split(",")
                };
            }

            if (rawToken.StartsWith("randombetween"))
            {
                this.logger.LogTrace("Mapping RandomBetween token");

                int start = rawToken.IndexOf("(") + 1;
                int end = rawToken.IndexOf(")");
                string[] items = rawToken.Substring(start, end - start).Split(",");

                return new RandomBetweenToken
                {
                    Lower = int.Parse(items[0]),
                    Upper = int.Parse(items[1])
                };
            }

            if (rawToken.StartsWith("linefromfile"))
            {
                this.logger.LogTrace("Mapping linefromfile token");

                int start = rawToken.IndexOf("(") + 1;
                int end = rawToken.IndexOf(")");
                string filePath = rawToken.Substring(start, end - start);
                List<string> lines = new List<string>();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        lines.Add(await reader.ReadLineAsync());
                    }
                }

                return new LineFromFileToken()
                {
                    Items = lines.ToArray()
                };
            }

            if(rawToken.StartsWith("variable"))
            {
                this.logger.LogTrace("Mapping variable token");

                int start = rawToken.IndexOf("(") + 1;
                int end = rawToken.IndexOf(")");
                string name = rawToken.Substring(start, end - start);

                return new VariableToken(name);
            }

            return null;

            // Exception exception = new NotImplementedException($"The token {rawToken} has not been implemented");
            // this.logger.LogError(exception, "Unable to map token");
            // throw exception;
        }
    }
}