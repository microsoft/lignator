// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
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
        private readonly IFileSink fileSink;
        private readonly ILogger<LogGenerator> logger;
        private readonly Random random;
        private Object randomLock;

        public LogGenerator(ITokenExtractor extractor, ITokenTransformer transformer, IFileSink fileSink, ILogger<LogGenerator> logger)
        {
            this.extractor = extractor;
            this.transformer = transformer;
            this.fileSink = fileSink;
            this.logger = logger;
            this.random = new Random();
            this.randomLock = new Object();
        }

        public async Task Generate(Options options)
        {
            if(!options.NoBanner) this.PrintBanner();

            this.WriteLineToConsole("Extracting Variables", ConsoleColor.Green);
            IDictionary<string, Extraction> variables = await this.ExtractVariables(options.Variable);
            this.WriteLineToConsole("Variables Extracted", ConsoleColor.Green);

            this.WriteLineToConsole("Start token extraction", ConsoleColor.Green);

            Extraction head = await this.GetFirstExtraction(options.Head, options.MultiLine);
            List<Extraction> extractions = await this.extractor.Extract(options.Template, options.MultiLine);
            Extraction tail = await this.GetFirstExtraction(options.Tail, options.MultiLine);

            this.WriteLineToConsole("Finished token extraction", ConsoleColor.Green);

            if (options.Infinite) this.WriteLineToConsole("infinite flag == true, will run until the process is stopped", ConsoleColor.Yellow);

            this.WriteLineToConsole("Start generation", ConsoleColor.Green);
            do
            {
                List<Task> tasks = new List<Task>();
                for (int n = 0; n < options.Runs; n++)
                {
                    ConcurrentBag<string> fileLocations = new ConcurrentBag<string>();
                    IEnumerable<IGrouping<string, Extraction>> grouping = extractions.GroupBy(x => x.SourceFileName);
                    foreach (IGrouping<string, Extraction> group in grouping)
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            string fileName = !string.IsNullOrEmpty(group.Key) ? group.Key : "lignator";
                            string path = $"{options.Output}/{fileName}.{options.Extension}";
                            fileLocations.Add(path);

                            int seed = 0;
                            lock (this.randomLock)
                            {
                                seed = this.random.Next();
                            }
                            Random taskRandom = new Random(seed);

                            using (IFileSink fileSink = this.fileSink.Start(path, options.MultiLine))
                            {
                                if (head != null) fileSink.Sink(this.transformer.Transform(head.Template, head.Tokens, taskRandom, variables));

                                this.WriteLineToConsole($"Starting run {n + 1} of {options.Runs}", ConsoleColor.Green);

                                List<Extraction> subGroup = group.ToList();
                                for (int l = 0; l < options.Logs; l++)
                                {
                                    Extraction extraction = subGroup[taskRandom.Next(0, subGroup.Count())];
                                    string transformed = this.transformer.Transform(extraction.Template, extraction.Tokens, taskRandom, variables);
                                    fileSink.Sink(transformed);
                                }

                                if (tail != null) fileSink.Sink(this.transformer.Transform(tail.Template, tail.Tokens, taskRandom, variables));
                            }

                            if (options.Clean)
                            {
                                this.fileSink.DeleteFile(path);
                            }
                        }));
                    }

                    await Task.WhenAll(tasks);
                }
            }
            while (options.Infinite);

            this.WriteLineToConsole("Finished generation", ConsoleColor.Green);
        }

        private async Task<Extraction> GetFirstExtraction(string template, bool multiLine)
        {
            if(string.IsNullOrWhiteSpace(template)) return null;
            List<Extraction> extractions = await this.extractor.Extract(template, multiLine);
            return extractions != null && extractions.Any() ? extractions.FirstOrDefault() : null;
        }
        private void WriteLineToConsole(string line, ConsoleColor text, bool log = true)
        {
            this.logger.LogTrace(line);

            ConsoleColor currentText = Console.ForegroundColor;
            Console.ForegroundColor = text;
            Console.WriteLine(line);
            Console.ForegroundColor = currentText;
        }

        private async Task<IDictionary<string, Extraction>> ExtractVariables(string[] variables)
        {
            if (variables == null || variables.Length < 1) return null;

            IDictionary<string, Extraction> extracted = new Dictionary<string, Extraction>();

            foreach(string variable in variables)
            {
                string[] parts = variable.Split("=");
                string name = parts[0];
                string rawTemplate = parts[1];

                Extraction extraction = await this.GetFirstExtraction(rawTemplate, false);
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