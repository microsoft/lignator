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
        static int Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("lignator_")
                .Build();

            ServiceProvider services = new ServiceCollection()
                .AddLogging(c =>
                {
                    c.AddConfiguration(configuration);
                    c.AddConsole();
                    c.AddEventSourceLogger();
                })
                .AddTransient<ITokenMapper, TokenMapper>()
                .AddTransient<ITokenExtractor, TokenExtractor>()
                .AddTransient<ITokenTransformer, TokenTransformer>()
                .AddTransient<ILogGenerator, LogGenerator>()
                .AddTransient<IFileSink, FileSink>()
                .BuildServiceProvider();

            string tempalteDefault = configuration.GetValue<string>("template");
            Option<string> template;

            if (!string.IsNullOrWhiteSpace(tempalteDefault))
            {
                template = new Option<string>(
                    new string[] { "--template", "-t" },
                    () => tempalteDefault,
                    "The template to be used for each log line or a file which contains a template per line which will be randomly selected for use")
                { IsRequired = true };
            }
            else
            {
                template = new Option<string>(
                    new string[] { "--template", "-t" },
                    "The template to be used for each log line")
                { IsRequired = true };
            }

            int runsDefault = configuration.GetValue<int>("runs");
            Option<int> runs = new Option<int>(
                    new string[] { "--runs", "-r" },
                    () => runsDefault == 0 ? 1 : runsDefault,
                    description: "How many runs?");

            string outputDefault = configuration.GetValue<string>("output");
            Option<string> outputOption = new Option<string>(
                    new string[] { "--output", "-o" },
                    () => string.IsNullOrWhiteSpace(outputDefault) ? "logs" : outputDefault,
                    description: "The directory you would like the logs to be put");

            int logsDefault = configuration.GetValue<int>("logs");
            Option<int> logs = new Option<int>(
                    new string[] { "--logs", "-l" },
                    () => logsDefault == 0 ? 1 : logsDefault,
                    description: "How many logs per run?");

            int threadsDefault = configuration.GetValue<int>("threads");
            Option<int> threadsOption = new Option<int>(
                    new string[] { "--threads" },
                    () => threadsDefault == 0 ? 1 : threadsDefault,
                    description: "How many threads per run?");

            Option<bool> infiniteOption = new Option<bool>(
                    new string[] { "--infinite", "-i" },
                    () => configuration.GetValue<bool>("infinite"),
                    description: "Run continuously?");

            Option<bool> cleanOption = new Option<bool>(
                    new string[] { "--clean", "-c" },
                    () => configuration.GetValue<bool>("clean"),
                    description: "Delete the files at the end of each run?");

            string extensionDefault = configuration.GetValue<string>("extension");
            Option<string> outputExtensionOption = new Option<string>(
                    new string[] { "--extension", "-e" },
                    () => string.IsNullOrWhiteSpace(extensionDefault) ? "log" : extensionDefault,
                    description: "The file extension used when generating the file outputs");

            Option<bool> multiLineOption = new Option<bool>(
                    new string[] { "--multi-line", "-m" },
                    () => configuration.GetValue<bool>("multiline"),
                    description: "Used with either file or directory input to use the whole file as a single input");

            string headDefault = configuration.GetValue<string>("head");
            Option<string> headOption = new Option<string>(
                    new string[] { "--head", "-H" },
                    () => headDefault,
                    description: "Add content to the start of the file");

            string tailDefault = configuration.GetValue<string>("tail");
            Option<string> tailOption = new Option<string>(
                    new string[] { "--tail", "-T" },
                    () => tailDefault,
                    description: "Add content to the end of the file");


            string variablesDefault = configuration.GetValue<string>("variables");
            Option<string[]> variablesOption = new Option<string[]>(
                    new string[] { "--variable", "-V" },
                    () => string.IsNullOrWhiteSpace(variablesDefault) ? null : variablesDefault.Split(";"), // env variable would be equal to "myid=%{uuid}%;currenttimestamp=%{utcnow()}%"
                    description: "Key value pairs for variable declaration, they can also be templates"
            );

            Option<bool> nobannerOption = new Option<bool>(
                    new string[] { "--no-banner" },
                    () => configuration.GetValue<bool>("no_banner"),
                    description: "Hide the ascii banner");

            string tokenOpening = configuration.GetValue<string>("token_opening", "${");
            Option<string> tokenOpeningOption = new Option<string>(
                    new string[] { "--token-opening" },
                    () => tokenOpening,
                    description: "The identifier used to show the start of a token or expression");

            string tokenClosing = configuration.GetValue<string>("token_closing", "}");
            Option<string> tokenClosingOption = new Option<string>(
                    new string[] { "--token-closing" },
                    () => tokenClosing,
                    description: "The identifier used to show the end of a token or expression");

            RootCommand command = new RootCommand
            {
                template,
                runs,
                outputOption,
                logs,
                threadsOption,
                infiniteOption,
                cleanOption,
                outputExtensionOption,
                multiLineOption,
                headOption,
                tailOption,
                variablesOption,
                nobannerOption,
                tokenOpeningOption,
                tokenClosingOption
            };

            command.Handler = CommandHandler.Create<Options>(services.GetService<ILogGenerator>().Generate);

            return command.InvokeAsync(args).Result;
        }
    }
}
