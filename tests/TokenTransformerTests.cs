// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Lignator.Interfaces;
using Lignator.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lignator.Tests
{
    public class TokenTransformerTests
    {
        [Fact]
        public void Transform_uuid_ToGuid()
        {
            // Arrange
            Mock<ILogger<TokenTransformer>> logger = new Mock<ILogger<TokenTransformer>>();
            ITokenTransformer transformer = new TokenTransformer(logger.Object);

            // Act
            string transformed = transformer.Transform("I am a {0}", new List<Token> { new UuidToken() }, new Random(), null);

            // Assert
            string uuid = transformed.Substring(7);
            Guid guid;
            Assert.True(Guid.TryParse(uuid, out guid));
        }

        [Fact]
        public void Transform_randomitem_ToRandomItemInArray()
        {
            // Arrange
            Token token = new RandomItemToken {
                Items = new [] { "Info", "Warn", "Error" }
            };
            Mock<ILogger<TokenTransformer>> logger = new Mock<ILogger<TokenTransformer>>();
            ITokenTransformer transformer = new TokenTransformer(logger.Object);

            // Act
            string transformed = transformer.Transform("I am a {0}", new List<Token> { token }, new Random(), null);

            // Assert
            string logLevel = transformed.Substring(7);
            Assert.Contains(logLevel, new [] { "Info", "Warn", "Error"});
        }


        [Fact]
        public void Transform_utcnow_ToUtcNow()
        {
            // Arrange
            UtcNowToken token = new UtcNowToken();
            Mock<ILogger<TokenTransformer>> logger = new Mock<ILogger<TokenTransformer>>();
            ITokenTransformer transformer = new TokenTransformer(logger.Object);

            // Act
            string transformed = transformer.Transform("I am a {0}", new List<Token> { token }, new Random(), null);

            // Assert
            string value = transformed.Substring(7);
            DateTime dateTime;
            Assert.True(DateTime.TryParse(value, out dateTime));
        }

        [Fact]
        public void Transform_utcnow_ToUtcNowShort()
        {
            // Arrange
            UtcNowToken token = new UtcNowToken("yyyy-MM-dd");
            Mock<ILogger<TokenTransformer>> logger = new Mock<ILogger<TokenTransformer>>();
            ITokenTransformer transformer = new TokenTransformer(logger.Object);

            // Act
            string transformed = transformer.Transform("I am a {0}", new List<Token> { token }, new Random(), null);

            // Assert
            string value = transformed.Substring(7);
            DateTime dateTime;
            Assert.True(DateTime.TryParse(value, out dateTime));
            Assert.Equal(DateTime.UtcNow.ToString("yyy-MM-dd"), value);
        }

        [Fact]
        public void Transform_randombetween_ToRandomNumberFromRange()
        {
            // Arrange
            Token token = new RandomBetweenToken
            {
                Lower = 1,
                Upper = 10
            };
            Mock<ILogger<TokenTransformer>> logger = new Mock<ILogger<TokenTransformer>>();
            ITokenTransformer transformer = new TokenTransformer(logger.Object);

            // Act
            string transformed = transformer.Transform("I am a {0}", new List<Token> { token }, new Random(), null);

            // Assert
            string numberAsString = transformed.Substring(7);
            int number;
            Assert.True(int.TryParse(numberAsString, out number));
            Assert.InRange<int>(number, 1, 10);
        }

        [Fact]
        public void Transform_linefromfile_ToItemInArray()
        {
            // Arrange
            Token token = new LineFromFileToken
            {
                Items = new[] { "emerg", "alert", "crit", "error", "warn", "notice" }
            };
            Mock<ILogger<TokenTransformer>> logger = new Mock<ILogger<TokenTransformer>>();
            ITokenTransformer transformer = new TokenTransformer(logger.Object);

            // Act
            string transformed = transformer.Transform("I am a {0}", new List<Token> { token }, new Random(), null);

            // Assert
            string logLevel = transformed.Substring(7);
            Assert.Contains(logLevel, new[] { "emerg", "alert", "crit", "error", "warn", "notice" });
        }

        [Fact]
        public void Transform_StringWithQuotes_CorrectlyFormated()
        {
            // Arrange
            Token token = new UuidToken();
            Mock<ILogger<TokenTransformer>> logger = new Mock<ILogger<TokenTransformer>>();
            ITokenTransformer transformer = new TokenTransformer(logger.Object);

            // Act
            string transformed = transformer.Transform("{{\"Hello\":\"{0}\"}}", new List<Token> { token }, new Random(), null);

            // Assert
            string uuid = transformed.Substring(10, transformed.Length - 12);
            Guid guid;
            Assert.True(Guid.TryParse(uuid, out guid));
        }

        [Fact]
        public void Transform_TemplateWithVariables_VariableUsed()
        {
            // Arrange
            Token token = new VariableToken("myid");
            Mock<ILogger<TokenTransformer>> logger = new Mock<ILogger<TokenTransformer>>();
            ITokenTransformer transformer = new TokenTransformer(logger.Object);

            // Act
            string transformed = transformer.Transform(
                "{{\"Hello\":\"{0}\"}}",
                new List<Token> { token }, new Random(), new Dictionary<string, Extraction> {{ "myid", new Extraction { Template = "ABC", Tokens = new List<Token>() }}});

            // Assert
            string id = transformed.Substring(10, transformed.Length - 12);
            Assert.Equal("ABC", id);
        }
    }
}