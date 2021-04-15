// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Lignator.Interfaces;
using Lignator.Models;

namespace Lignator
{
    class Program
    {
        static IConfiguration configuration;
        static int Main(string[] args)
        {
            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables("lignator_")
                .Build();

            LogLevel logLevel = configuration.GetValue<string>("log_level", "Information") == "Information" ? LogLevel.Information : LogLevel.None;
            ServiceProvider services = new ServiceCollection()
                .AddLogging(c =>
                {
                    c.AddConfiguration(configuration);
                    c.AddEventSourceLogger();
                    c.AddFilter("Lignator.LogGenerator", logLevel);
                    c.AddFilter("Default", LogLevel.Information);
                    c.AddFilter("Microsoft", LogLevel.Warning);
                    c.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
                    c.AddConsole();
                })
                .AddTransient<ITokenMapper, TokenMapper>()
                .AddTransient<ITokenExtractor, TokenExtractor>()
                .AddTransient<ITokenTransformer, TokenTransformer>()
                .AddTransient<ILogGenerator, LogGenerator>()
                .AddTransient<ISink, QueueSink>()
                .BuildServiceProvider();

            string variablesDefault = configuration.GetValue<string>("variables");
            Option<string[]> variablesOption = new Option<string[]>(
                    new string[] { "--variable", "-V" },
                    () => string.IsNullOrWhiteSpace(variablesDefault) ? null : variablesDefault.Split(";"), // env variable would be equal to "myid=%{uuid}%;currenttimestamp=%{utcnow()}%"
                    description: "Key value pairs for variable declaration, they can also be templates");

            RootCommand command = new RootCommand
            {
                GetOption<string>("template", null, new string[] { "--template", "-t" }, "The template to be used for each log line or a file which contains a template per line which will be randomly selected for use"),
                GetOption<int>("runs", 1, new string[] { "--runs", "-r"}, "How many runs?"),
                GetOption<string>("output", "logs", new string[] { "--output", "-o" }, "The directory you would like the logs to be put, can also be '/dev/stdout"),
                GetOption<int>("logs", 1, new string[] { "--logs", "-l" }, "How many logs per run?"),
                GetOption<int>("threads", 1, new string[] { "--threads" }, "How many threads per run?"),
                GetOption<bool>("infinite", false, new string[] { "--infinite", "-i" }, "Run continuously?"),
                GetOption<bool>("clean", false, new string[] { "--clean", "-c" }, "Delete the files at the end of each run?"),
                GetOption<string>("extension", "log", new string[] { "--extension", "-e" }, "The file extension used when generating the file outputs"),
                GetOption<bool>("multiline", false, new string[] { "--multi-line", "-m" }, "Used with either file or directory input to use the whole file as a single input"),
                GetOption<string>("head", null, new string[] { "--head", "-H" }, "Add content to the start of the file"),
                GetOption<string>("tail", null, new string[] { "--tail", "-T" }, "Add content to the end of the file"),
                variablesOption,
                GetOption<bool>("no_banner", false, new string[] { "--no-banner" }, "Hide the ascii banner"),
                GetOption<string>("token_opening", "${", new string[] { "--token-opening" }, "The identifier used to show the start of a token or expression"),
                GetOption<string>("token_closing", "}", new string[] { "--token-closing" }, "The identifier used to show the end of a token or expression")
            };

            command.Handler = CommandHandler.Create<Options>(services.GetService<ILogGenerator>().Generate);

            return command.InvokeAsync(args).Result;
        }

        private static Option<T> GetOption<T>(string environmentName, T defaultValue, string[] aliases, string description)
        {
            return defaultValue != null
                ? new Option<T>(aliases, () => configuration.GetValue<T>(environmentName, defaultValue), description)
                : new Option<T>(aliases, description);
        }
    }
}
