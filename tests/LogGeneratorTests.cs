// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lignator.Interfaces;
using Lignator.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lignator.Tests
{
    public class LogGeneratorTests
    {
        [Fact]
        public async Task Generate_WithValidParams_CallsExtractor()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_WithValidParams_CallsExtractor";
            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            extractor.Setup(o => o.Extract(It.IsAny<string>(), false)).ReturnsAsync(new List<Extraction> { extraction });

            Mock<ITokenTransformer> transformer = new Mock<ITokenTransformer>();
            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fileSink.Object);

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer.Object, fileSink.Object, logger.Object);
            string template = "Hello %{randomitem(world,universe)}%";

            // Act
            Options options = new Options {
                Template = template,
                Clean = true
            };
            await generator.Generate(options);

            // Assert
            extractor.Verify(o => o.Extract(template, false));
        }

        [Fact]
        public async Task Generate_WithValidParams_CallsTransformer()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_WithValidParams_CallsTransformer";
            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            extractor.Setup(o => o.Extract(It.IsAny<string>(), false)).ReturnsAsync(new List<Extraction> { extraction });

            Mock<ITokenTransformer> transformer = new Mock<ITokenTransformer>();
            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fileSink.Object);

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer.Object, fileSink.Object, logger.Object);
            string template = "Hello %{randomitem(world,universe)}%";

            // Act
            Options options = new Options {
                Template = template,
                Clean = true
            };
            await generator.Generate(options);

            // Assert
            transformer.Verify(o => o.Transform(extraction.Template, extraction.Tokens, It.IsAny<Random>(), null));
        }

        [Fact]
        public async Task Generate_WithValidParams_CallsTransformerPerRun()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_WithValidParams_CallsTransformerPerRun";
            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            string template = "Hello %{randomitem(world,universe)}%";
            extractor.Setup(o => o.Extract(template, false))
                .ReturnsAsync(new List<Extraction> { extraction });
            extractor.Setup(o => o.Extract(null, false))
                .Returns<List<Extraction>>(default);

            Mock<ITokenTransformer> transformer = new Mock<ITokenTransformer>();
            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fileSink.Object);

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer.Object, fileSink.Object, logger.Object);

            // Act
            Options options = new Options {
                Template = template,
                Runs = 5,
                Clean = true
            };
            await generator.Generate(options);

            // Assert
            transformer.Verify(o => o.Transform(extraction.Template, extraction.Tokens, It.IsAny<Random>(), null), Times.Exactly(5));
        }

        [Fact]
        public async Task Generate_WithValidParams_CallsTransformerPerRunPerLog()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_WithValidParams_CallsTransformerPerRunPerLog";
            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            string template = "Hello %{randomitem(world,universe)}%";
            extractor.Setup(o => o.Extract(template, false))
                .ReturnsAsync(new List<Extraction> { extraction });
            extractor.Setup(o => o.Extract(null, false))
                .Returns<List<Extraction>>(default);

            Mock<ITokenTransformer> transformer = new Mock<ITokenTransformer>();
            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fileSink.Object);

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer.Object, fileSink.Object, logger.Object);

            // Act
            Options options = new Options {
                Template = template,
                Runs = 5,
                Logs = 5,
                Clean = true
            };
            await generator.Generate(options);

            // Assert
            transformer.Verify(o => o.Transform(
                It.Is<string>(s => s == extraction.Template),
                It.IsAny<IEnumerable<Token>>(),
                It.IsAny<Random>(),
                null),
                Times.Exactly(25));
        }

        [Fact]
        public async Task Generate_WithValidParams_CallsTransformerOnceForAllRunsAndLogs()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_WithValidParams_CallsTransformerOnceForAllRunsAndLogs";
            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            extractor.Setup(o => o.Extract(It.IsAny<string>(), false)).ReturnsAsync(new List<Extraction> { extraction });

            Mock<ITokenTransformer> transformer = new Mock<ITokenTransformer>();
            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fileSink.Object);

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer.Object, fileSink.Object, logger.Object);
            string template = "Hello %{randomitem(world,universe)}%";

            // Act
            Options options = new Options {
                Template = template,
                Runs = 5,
                Logs = 5,
                Clean = true
            };
            await generator.Generate(options);

            // Assert
            extractor.Verify(o => o.Extract(template, false), Times.Exactly(1));
        }

        [Fact]
        public async Task Generate_ExtractionWithSourceName_CreateFileIncludesItInOutputFile()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_ExtractionWithSourceName_CreateFileIncludesItInOutputFile";
            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            extractor.Setup(o => o.Extract(It.IsAny<string>(), false)).ReturnsAsync(new List<Extraction> { extraction });

            Mock<ITokenTransformer> transformer = new Mock<ITokenTransformer>();
            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fileSink.Object);

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer.Object, fileSink.Object, logger.Object);
            string template = "Hello %{randomitem(world,universe)}%";

            // Act
            Options options = new Options {
                Template = template,
                Runs = 1,
                Logs = 1,
                Clean = false
            };
            await generator.Generate(options);

            // Assert
            fileSink.Verify(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Exactly(1));
        }

        [Fact]
        public async Task Generate_ExtractionWithNewLine_CreateLogWithMultipleLines()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_ExtractionWithNewLine_CreateLogWithMultipleLines";

            string template = string.Format("Hello{0}{1}", Environment.NewLine, "{0}");
            Extraction extraction = new Extraction {
                Template = template,
                Tokens = new List<Token> {
                    new RandomItemToken { Items = new [] { "world", "universe" } }
                },
                SourceFileName = sourceFile
            };

            extractor.Setup(o => o.Extract(It.IsAny<string>(), true)).ReturnsAsync(new List<Extraction> { extraction });

            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            ITokenTransformer transformer = new TokenTransformer(new Mock<ILogger<TokenTransformer>>().Object);
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            IFileSink instance = fileSink.Object;
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(instance);

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer, instance, logger.Object);

            // Act
            Options options = new Options {
                Template = template,
                Clean = false,
                MultiLine = true
            };
            await generator.Generate(options);

            // Assert;
            fileSink.Verify(o => o.Sink(It.Is<string>(s => s == $"Hello{Environment.NewLine}world" || s == $"Hello{Environment.NewLine}universe")), Times.Exactly(1));
            fileSink.Verify(o => o.Sink(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public async Task Generate_ExtractionWithHead_CreateLogWithHead()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_ExtractionWithHead_CreateLogWithHead";

            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            string template = "Hello %{randomitem(world,universe)}%";
            string head = "Say Hello!";
            extractor.Setup(o => o.Extract(template, false))
                .ReturnsAsync(new List<Extraction> { extraction });
            extractor.Setup(o => o.Extract(head, false))
                .ReturnsAsync(new List<Extraction> { new Extraction { Template = head, Tokens = new List<Token>() }});
            extractor.Setup(o => o.Extract(null, false))
                .Returns<List<Extraction>>(default);

            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            ITokenTransformer transformer = new TokenTransformer(new Mock<ILogger<TokenTransformer>>().Object);
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fileSink.Object);
            MockSequence sequence = new MockSequence();
            fileSink.InSequence(sequence).Setup(o => o.Sink("Say Hello!"));
            fileSink.InSequence(sequence).Setup(o => o.Sink(It.Is<string>(s => s == "Hello world" || s == "Hello universe")));

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer, fileSink.Object, logger.Object);

            // Act
            Options options = new Options {
                Template = template,
                Clean = false,
                MultiLine = false,
                Head = head
            };
            await generator.Generate(options);

            // Assert
            fileSink.Verify(o => o.Sink("Say Hello!"), Times.Exactly(1));
            fileSink.Verify(o => o.Sink(It.Is<string>(s => s == "Hello world" || s == "Hello universe")), Times.Exactly(1));
            fileSink.Verify(o => o.Sink(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Generate_ExtractionWithTail_CreateLogWithTail()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_ExtractionWithTail_CreateLogWithTail";

            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            string template = "Hello %{randomitem(world,universe)}%";
            string tail = "Say Hello!";
            extractor.Setup(o => o.Extract(template, false))
                .ReturnsAsync(new List<Extraction> { extraction });
            extractor.Setup(o => o.Extract(tail, false))
                .ReturnsAsync(new List<Extraction> { new Extraction { Template = tail, Tokens = new List<Token>() }});
            extractor.Setup(o => o.Extract(null, false))
                .Returns<List<Extraction>>(default);

            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            ITokenTransformer transformer = new TokenTransformer(new Mock<ILogger<TokenTransformer>>().Object);
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fileSink.Object);
            MockSequence sequence = new MockSequence();
            fileSink.InSequence(sequence).Setup(o => o.Sink(It.Is<string>(s => s == "Hello world" || s == "Hello universe")));
            fileSink.InSequence(sequence).Setup(o => o.Sink("Say Hello!"));

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer, fileSink.Object, logger.Object);

            // Act
            Options options = new Options {
                Template = template,
                Clean = false,
                MultiLine = false,
                Tail = tail
            };
            await generator.Generate(options);

            // Assert
            fileSink.Verify(o => o.Sink("Say Hello!"), Times.Exactly(1));
            fileSink.Verify(o => o.Sink(It.Is<string>(s => s == "Hello world" || s == "Hello universe")), Times.Exactly(1));
            fileSink.Verify(o => o.Sink(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Generate_ExtractionWithHeadAndTail_CreateLogWithHeadAndTail()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_ExtractionWithHeadAndTail_CreateLogWithHeadAndTail";

            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            string template = "Hello %{randomitem(world,universe)}%";
            string head = "Say Hello!";
            string tail = "Say Bye!";
            extractor.Setup(o => o.Extract(template, false))
                .ReturnsAsync(new List<Extraction> { extraction });
            extractor.Setup(o => o.Extract(head, false))
                .ReturnsAsync(new List<Extraction> { new Extraction { Template = head, Tokens = new List<Token>() }});
            extractor.Setup(o => o.Extract(tail, false))
                .ReturnsAsync(new List<Extraction> { new Extraction { Template = tail, Tokens = new List<Token>() }});

            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            ITokenTransformer transformer = new TokenTransformer(new Mock<ILogger<TokenTransformer>>().Object);
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            IFileSink instance = fileSink.Object;
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(instance);
            MockSequence sequence = new MockSequence();
            fileSink.InSequence(sequence).Setup(o => o.Sink("Say Hello!"));
            fileSink.InSequence(sequence).Setup(o => o.Sink(It.Is<string>(s => s == "Hello world" || s == "Hello universe")));
            fileSink.InSequence(sequence).Setup(o => o.Sink("Say Bye!"));

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer, instance, logger.Object);

            // Act
            Options options = new Options {
                Template = template,
                Clean = false,
                MultiLine = false,
                Head = head,
                Tail = tail
            };
            await generator.Generate(options);

            // Assert
            fileSink.Verify(o => o.Sink("Say Hello!"), Times.Exactly(1));
            fileSink.Verify(o => o.Sink(It.Is<string>(s => s == "Hello world" || s == "Hello universe")), Times.Exactly(1));
            fileSink.Verify(o => o.Sink("Say Bye!"), Times.Exactly(1));
            fileSink.Verify(o => o.Sink(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Generate_ExtractionWithHeadAndTailTransforms_CreateLogWithHeadAndTail()
        {
            // Arrange
            Mock<ITokenExtractor> extractor = new Mock<ITokenExtractor>();
            string sourceFile = "Generate_ExtractionWithHeadAndTailTransforms_CreateLogWithHeadAndTail";

            Extraction extraction = new Extraction {
                    SourceFileName = sourceFile,
                    Template = "Hello {0}",
                    Tokens = new List<Token> {
                        new RandomItemToken { Items = new [] { "world", "universe" } }
                        }
                    };

            string template = "Hello %{randomitem(world,universe)}%";
            string head = "Head: {0}";
            string tail = "Tail: {0}";
            extractor.Setup(o => o.Extract(template, false))
                .ReturnsAsync(new List<Extraction> { extraction });
            extractor.Setup(o => o.Extract(head, false))
                .ReturnsAsync(new List<Extraction> { new Extraction { Template = head, Tokens = new List<Token> { new RandomItemToken { Items = new [] { "Hi", "Hello" }}} }});
            extractor.Setup(o => o.Extract(tail, false))
                .ReturnsAsync(new List<Extraction> { new Extraction { Template = tail, Tokens = new List<Token> { new RandomItemToken { Items = new [] { "Bye", "Goodbye" }}} }});

            Mock<ILogger<LogGenerator>> logger = new Mock<ILogger<LogGenerator>>();
            ITokenTransformer transformer = new TokenTransformer(new Mock<ILogger<TokenTransformer>>().Object);
            Mock<IFileSink> fileSink = new Mock<IFileSink>();
            fileSink.Setup(o => o.Start(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fileSink.Object);
            MockSequence sequence = new MockSequence();
            fileSink.InSequence(sequence).Setup(o => o.Sink(It.Is<string>(s => s == "Head: Hi" || s == "Head: Hello")));
            fileSink.InSequence(sequence).Setup(o => o.Sink(It.Is<string>(s => s == "Hello world" || s == "Hello universe")));
            fileSink.InSequence(sequence).Setup(o => o.Sink(It.Is<string>(s => s == "Tail: Bye" || s == "Tail: Goodbye")));

            ILogGenerator generator = new LogGenerator(extractor.Object, transformer, fileSink.Object, logger.Object);

            // Act
            Options options = new Options {
                Template = template,
                Clean = false,
                MultiLine = false,
                Head = head,
                Tail = tail
            };
            await generator.Generate(options);

            // Assert
            fileSink.Verify(o => o.Sink(It.Is<string>(s => s == "Head: Hi" || s == "Head: Hello")));
            fileSink.Verify(o => o.Sink(It.Is<string>(s => s == "Hello world" || s == "Hello universe")));
            fileSink.Verify(o => o.Sink(It.Is<string>(s => s == "Tail: Bye" || s == "Tail: Goodbye")));
            fileSink.Verify(o => o.Sink(It.IsAny<string>()), Times.Exactly(3));
        }
    }
}