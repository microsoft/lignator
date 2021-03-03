using System;
using System.Linq;
using Xunit;
using Lignator.Interfaces;
using Lignator.Models;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Lignator.Tests
{
    public class TokenExtractorTests
    {
        [Fact]
        public async Task Extract_TemplateWithOneToken_TokenExtracted()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("I am a log for %{uuid}%");

            // Assert
            Extraction extraction = extractions.First();
            Assert.Equal("I am a log for {0}", extraction.Template);
            Assert.Equal("uuid", extraction.Tokens.First().Key);

        }

        [Fact]
        public async Task Extract_TemplateWithTwoToken_TokensExtracted()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("I am a log for %{uuid}% about %{uuid}%");

            // Assert
            Extraction extraction = extractions.First();
            Assert.Equal("I am a log for {0} about {1}", extraction.Template);
            Assert.Equal("uuid", extraction.Tokens[0].Key);
            Assert.Equal("uuid", extraction.Tokens[1].Key);

        }


        [Fact]
        public async Task Extract_TemplateWithTwoTokenInJson_TokensExtracted()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("{\"Hello\":\"%{randomitem(world,universe)}%\", \"id\": \"%{uuid}%\"}");

            // Assert
            Extraction extraction = extractions.First();
            Assert.Equal("{{\"Hello\":\"{0}\", \"id\": \"{1}\"}}", extraction.Template);
            Assert.Equal("randomitem", extraction.Tokens[0].Key);
            Assert.Equal("uuid", extraction.Tokens[1].Key);
        }

        [Fact]
        public async Task Extract_TemplatesInFile_ExtractionPerTemplate()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("./Samples/Extract_TemplatesInFile_ExtractionPerTemplate.template");

            // Assert
            Assert.Equal(2, extractions.Count);
            Assert.Equal("Hello {0}", extractions[0].Template);
            Assert.Equal("randomitem", extractions[0].Tokens[0].Key);
            Assert.Equal("Id: {0}", extractions[1].Template);
            Assert.Equal("uuid", extractions[1].Tokens[0].Key);
        }

        [Fact]
        public async Task Extract_TemplatesInDirectory_ExtractionPerTemplate()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("./Samples");

            // Assert
            Assert.Equal(4, extractions.Count);
            Assert.Contains(extractions, x => x.Template == "Hello {0}" && x.Tokens[0].Key == "randomitem");
            Assert.Contains(extractions, x => x.Template == "Uuid: {0}" && x.Tokens[0].Key == "uuid");
            Assert.Contains(extractions, x => x.Template == "Id: {0}" && x.Tokens[0].Key == "uuid");
            Assert.Contains(extractions, x => x.Template == "Timestamp {0}" && x.Tokens[0].Key == "utcnow");
        }

        [Fact]
        public async Task Extract_TemplatesInDirectory_ExtractionPerTemplateWithFileName()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("./Samples");

            // Assert
            Assert.Equal(2, extractions.Where(x => x.SourceFileName == "Extract_TemplatesInFile_ExtractionPerTemplate").Count());
            Assert.Equal(2, extractions.Where(x => x.SourceFileName == "Extract_TemplatesInDirectory_ExtractionPerTemplate").Count());
        }

        [Fact]
        public async Task Extract_TemplatesWithQuotes_ExtractionSuccessfull()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("\"Hello\":\"%{uuid}%\"");

            // Assert
            Assert.Single(extractions);
            Assert.Equal("\"Hello\":\"{0}\"", extractions[0].Template);
            Assert.Equal("uuid", extractions[0].Tokens[0].Key);
        }

        [Fact]
        public async Task Extract_TemplatesWithSpecialCharacters_ExtractionSuccessfull()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("Hello\n%{uuid}%");

            // Assert
            Assert.Single(extractions);
            Assert.Equal("Hello\n{0}", extractions[0].Template);
            Assert.Equal("uuid", extractions[0].Tokens[0].Key);
        }

        [Fact]
        public async Task Extract_TemplatesWithJson_ExtractionSuccessfull()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("{\"Hello\": \"World\", \"id\": \"%{uuid}%\"}");

            // Assert
            Assert.Single(extractions);
            Assert.Equal("{{\"Hello\": \"World\", \"id\": \"{0}\"}}", extractions[0].Template);
            Assert.Equal("uuid", extractions[0].Tokens[0].Key);
        }

        [Fact]
        public async Task Extract_TemplateMultipleLines_TokenExtracted()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("./Samples/basic_mutliline.txt", true);

            // Assert
            Extraction extraction = extractions.First();
            Assert.Equal($"Hello{Environment.NewLine}World", extraction.Template);
        }

        [Fact]
        public async Task Extract_TemplateMultipleLinesInJson_TokensExtracted()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            Mock<ILogger<TokenExtractor>> extractionLogger = new Mock<ILogger<TokenExtractor>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            ITokenExtractor tokenExtractor = new TokenExtractor(mapper, extractionLogger.Object);

            // Act
            List<Extraction> extractions = await tokenExtractor.Extract("./Samples/multiline.json", true);

            // Assert
            Extraction extraction = extractions.First();
            Assert.Equal("{{\n    \"{0}\": \"{1}\"\n}}", extraction.Template);
        }
    }
}
