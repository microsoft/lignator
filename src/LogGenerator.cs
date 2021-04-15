// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lignator.Interfaces;
using Lignator.Models;
using Microsoft.Extensions.Logging;

namespace Lignator
{
    public class LogGenerator : ILogGenerator
    {
        private readonly ITokenExtractor extractor;
        private readonly ITokenTransformer transformer;
        private readonly ISink sink;
        private readonly ILogger<LogGenerator> logger;
        private readonly Random random;
        private Object randomLock;

        public LogGenerator(ITokenExtractor extractor, ITokenTransformer transformer, ISink sink, ILogger<LogGenerator> logger)
        {
            this.extractor = extractor;
            this.transformer = transformer;
            this.sink = sink;
            this.logger = logger;
            this.random = new Random();
            this.randomLock = new Object();
        }

        public async Task Generate(Options options)
        {
            if(!options.NoBanner) this.PrintBanner();

            this.logger.LogInformation("Extracting Variables");
            IDictionary<string, Extraction> variables = await this.ExtractVariables(options.Variable);
            this.logger.LogInformation("Variables Extracted");

            this.logger.LogInformation("Start token extraction");

            Extraction head = await this.GetFirstExtraction(options.Head, options.MultiLine, tokenOpening: options.TokenOpening, tokenClosing: options.TokenClosing);
            List<Extraction> extractions = await this.extractor.Extract(options.Template, options.MultiLine, tokenOpening: options.TokenOpening, tokenClosing: options.TokenClosing);
            Extraction tail = await this.GetFirstExtraction(options.Tail, options.MultiLine, tokenOpening: options.TokenOpening, tokenClosing: options.TokenClosing);

            this.logger.LogInformation("Finished token extraction");

            if (options.Infinite) this.logger.LogInformation("infinite flag == true, will run until the process is stopped");

            this.logger.LogInformation("Start generation");
            do
            {
                List<Task> tasks = new List<Task>();
                for (int n = 0; n < options.Runs; n++)
                {
                    IEnumerable<IGrouping<string, Extraction>> grouping = extractions.GroupBy(x => x.SourceFileName);
                    foreach (IGrouping<string, Extraction> group in grouping)
                    {
                        var task = Task.Run(() =>
                        {
                            string fileName = !string.IsNullOrEmpty(group.Key) ? group.Key : "lignator";
                            string path = options.Output == "/dev/stdout" ? options.Output : $"{options.Output}/{fileName}.{options.Extension}";

                            int seed = 0;
                            lock (this.randomLock)
                            {
                                seed = this.random.Next();
                            }
                            Random taskRandom = new Random(seed);

                            using (ISink sink = this.sink.Start(path, options.MultiLine, options.Clean))
                            {
                                if (head != null) sink.Sink(this.transformer.Transform(head.Template, head.Tokens, taskRandom, variables));

                                this.logger.LogInformation($"Starting run {n + 1} of {options.Runs}");

                                List<Extraction> subGroup = group.ToList();
                                for (int l = 0; l < options.Logs; l++)
                                {
                                    Extraction extraction = subGroup[taskRandom.Next(0, subGroup.Count())];
                                    string transformed = this.transformer.Transform(extraction.Template, extraction.Tokens, taskRandom, variables);
                                    sink.Sink(transformed);
                                }

                                if (tail != null) sink.Sink(this.transformer.Transform(tail.Template, tail.Tokens, taskRandom, variables));
                            }
                        });

                        if(options.Output == "/dev/stdout")
                        {
                            await task;
                        }
                        else
                        {
                            tasks.Add(task);
                        }
                    }

                    await Task.WhenAll(tasks);
                }
            }
            while (options.Infinite);

            this.logger.LogInformation("Finished generation");
        }

        private async Task<Extraction> GetFirstExtraction(string template, bool multiLine, string tokenOpening = "${", string tokenClosing = "}")
        {
            if(string.IsNullOrWhiteSpace(template)) return null;
            List<Extraction> extractions = await this.extractor.Extract(template, multiLine, tokenOpening, tokenClosing);
            return extractions != null && extractions.Any() ? extractions.FirstOrDefault() : null;
        }

        private async Task<IDictionary<string, Extraction>> ExtractVariables(string[] variables, string tokenOpening = "${", string tokenClosing = "}")
        {
            if (variables == null || variables.Length < 1) return null;

            IDictionary<string, Extraction> extracted = new Dictionary<string, Extraction>();

            foreach(string variable in variables)
            {
                string[] parts = variable.Split("=");
                string name = parts[0];
                string rawTemplate = parts[1];

                Extraction extraction = await this.GetFirstExtraction(rawTemplate, false, tokenOpening, tokenClosing);
                extracted.Add(name, extraction);
            }
            return extracted;
        }

        private void PrintBanner()
        {
            ConsoleColor currentText = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;

            // lignator
            Console.WriteLine("                                                      ");
            Console.WriteLine("                                                      ");
            Console.WriteLine("####    ###                                           ");
            Console.WriteLine("####   #####                                          ");
            Console.WriteLine("####    ###                        ##                 ");
            Console.WriteLine(" ####                              ##                 ");
            Console.WriteLine(" ####  #####                      #####               ");
            Console.WriteLine(" ####  #####                      #####               ");
            Console.WriteLine(" ####   ###                        ##                 ");
            Console.WriteLine(" ####   ###                        ##                 ");
            Console.WriteLine(" ####   ###    ###  ##       ###   ##      ##   ##    ");
            Console.WriteLine(" ####   ###   ##### ######  #####  ##     ####  ######");
            Console.WriteLine(" ####   ###  ##  ## ##  ## ##  ##  ##    ##  ## ##  ##");
            Console.WriteLine(" ####   ###  ##  ## ##  ## ##  ##  ##    ##  ## ##    ");
            Console.WriteLine(" ####   ###  ##  ## ##  ## ##  ##  ##    ##  ## ##    ");
            Console.WriteLine("  ####  ###  ##  ## ##  ## ##  ##  ##    ##  ## ##    ");
            Console.WriteLine("  #### #####  ##### ##  ##  #####   ####  ####  ##    ");
            Console.WriteLine("  #### #####   #### ##  ##   ####   ####   ##   ##    ");
            Console.WriteLine("                 ##                                   ");
            Console.WriteLine("             ##  ##                                   ");
            Console.WriteLine("             ######                                   ");
            Console.WriteLine("              ###                                     ");
            Console.WriteLine("                                                      ");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("       S T R U C T U R E D  R A N D O M N E S S       ");
            Console.WriteLine("------------------------------------------------------");

            Console.ForegroundColor = currentText;
        }
    }
}