// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
            List<Extraction> extractions = await tokenExtractor.Extract("I am a log for ${uuid}");

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
            List<Extraction> extractions = await tokenExtractor.Extract("I am a log for ${uuid} about ${uuid}");

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
            List<Extraction> extractions = await tokenExtractor.Extract("{\"Hello\":\"${randomitem(world,universe)}\", \"id\": \"${uuid}\"}");

            // Assert
            Extraction extraction = extractions.First();
            Assert.Equal("{{\"Hello\":\"{0}\", \"id\": \"{1}\"}}", extraction.Template);
            Assert.Equal("randomitem", extraction.Tokens[0].Key);
            Assert.Equal("uuid", extraction.Tokens[1].Key);
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
            List<Extraction> extractions = await tokenExtractor.Extract("\"Hello\":\"${uuid}\"");

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
            List<Extraction> extractions = await tokenExtractor.Extract("Hello\n${uuid}");

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
            List<Extraction> extractions = await tokenExtractor.Extract("{\"Hello\": \"World\", \"id\": \"${uuid}\"}");

            // Assert
            Assert.Single(extractions);
            Assert.Equal("{{\"Hello\": \"World\", \"id\": \"{0}\"}}", extractions[0].Template);
            Assert.Equal("uuid", extractions[0].Tokens[0].Key);
        }
    }
}
