// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using System.Collections.Generic;
using Lignator.Interfaces;
using Lignator.Models;
using Lignator.Helpers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;

namespace Lignator
{
    public class TokenExtractor : ITokenExtractor
    {
        private readonly ITokenMapper mapper;
        private readonly ILogger<TokenExtractor> logger;

        public TokenExtractor(ITokenMapper mapper, ILogger<TokenExtractor> logger)
        {
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task<List<Extraction>> Extract(string template, bool multiLine = false, string tokenOpening = "${", string tokenClosing = "}")
        {
            if(string.IsNullOrWhiteSpace(template)) return null;

            this.logger.LogTrace("Extracting tokens from template");

            List<Extraction> extractions = new List<Extraction>();

            if (File.Exists(template))
            {
                extractions = await this.ExtractTemplateFile(template, multiLine);
            }
            else if (Directory.Exists(template))
            {
                string[] files = Directory.GetFiles(template, "*.template");
                foreach (string file in files)
                {
                    extractions.AddRange(await this.ExtractTemplateFile(file));
                }
            }
            else
            {
                // if template is not a file or directory treat it as an inline template
                extractions.Add(new Extraction { UnProcessedTemplate = template });
            }

            string escapedTokenOpening = tokenOpening.Replace("{", "{{");
            string escapedTokenClosing = tokenClosing.Replace("}", "}}");

            foreach (Extraction extraction in extractions)
            {
                // escape braces for things like json
                extraction.UnProcessedTemplate = extraction.UnProcessedTemplate.Replace("{", "{{");
                extraction.UnProcessedTemplate = extraction.UnProcessedTemplate.Replace("}", "}}");

                MatchCollection matches = Regex.Matches(extraction.UnProcessedTemplate, $@"\{escapedTokenOpening}(.*?)\{escapedTokenClosing}");

                extraction.Tokens = new List<Token>();
                extraction.Template = extraction.UnProcessedTemplate;

                for (int n = 0; n < matches.Count; n++)
                {
                    extraction.Template = extraction.Template.ReplaceFirst(matches[n].Value, "{" + n + "}");
                    string rawToken = matches[n].Value.Substring(escapedTokenOpening.Length, matches[n].Value.Length - (escapedTokenOpening.Length + escapedTokenClosing.Length));
                    extraction.Tokens.Add(await this.mapper.MapToTokenAsync(rawToken));
                }
                this.logger.LogTrace($"{matches.Count} tokens extracted from template");
            }

            return extractions;
        }

        private async Task<List<Extraction>> ExtractTemplateFile(string file, bool multiLine = false)
        {
            List<Extraction> extractions = new List<Extraction>();
            using (StreamReader reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    Extraction extraction = new Extraction
                    {
                        SourceFileName = Path.GetFileName(file).Replace(".template", ""),
                        UnProcessedTemplate = multiLine ? await reader.ReadToEndAsync() : await reader.ReadLineAsync()
                    };
                    extractions.Add(extraction);
                }
            }
            return extractions;
        }
    }
}