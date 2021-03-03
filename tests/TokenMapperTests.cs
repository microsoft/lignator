using Lignator.Interfaces;
using Lignator.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Lignator.Tests
{
    public class TokenMapperTests
    {
        [Fact]
        public async Task MapToToken_uuid_UuidToken()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);

            // Act
            Token token = await mapper.MapToTokenAsync("uuid");

            // Assert
            UuidToken uuidToken = Assert.IsAssignableFrom<UuidToken>(token);
        }

        [Fact]
        public async Task MapToToken_randomitem_RandomItem()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);

            // Act
            Token token = await mapper.MapToTokenAsync("randomitem(info,warn,error)");

            // Assert
            RandomItemToken randomItemToken = Assert.IsAssignableFrom<RandomItemToken>(token);
            Assert.Equal(new [] { "info", "warn", "error" }, randomItemToken.Items);
        }

        [Fact]
        public async Task MapToToken_utcnow_UtcNowToken()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);

            // Act
            Token token = await mapper.MapToTokenAsync("utcnow(yyyy-MM-dd)");

            // Assert
            UtcNowToken uuidToken = Assert.IsAssignableFrom<UtcNowToken>(token);
        }

        [Fact]
        public async Task MapToToken_utcnow_UtcNowTokenWithDefaultFormat()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);

            // Act
            Token token = await mapper.MapToTokenAsync("utcnow()");

            // Assert
            UtcNowToken uuidToken = Assert.IsAssignableFrom<UtcNowToken>(token);
        }

        [Fact]
        public async Task MapToToken_randombetween_RandomBetweenToken()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);

            // Act
            Token token = await mapper.MapToTokenAsync("randombetween(1,10)");

            // Assert
            RandomBetweenToken randomBetweenToken = Assert.IsAssignableFrom<RandomBetweenToken>(token);
            Assert.Equal(1, randomBetweenToken.Lower);
            Assert.Equal(10, randomBetweenToken.Upper);
        }

        [Fact]
        public async Task MapToToken_linefromfile_LineFromFileToken()
        {

            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);
            string path = @"Samples/accesslog_levels.txt";

            // Act
            Token token = await mapper.MapToTokenAsync("linefromfile(" + path + ")");

            // Assert
            LineFromFileToken lineFromFileToken = Assert.IsAssignableFrom<LineFromFileToken>(token);
            Assert.Equal(new[] { "emerg", "alert", "crit", "error", "warn", "notice" }, lineFromFileToken.Items);

        }

        [Fact]
        public async Task MapToToken_variable_VariableToken()
        {
            // Arrange
            Mock<ILogger<TokenMapper>> logger = new Mock<ILogger<TokenMapper>>();
            ITokenMapper mapper = new TokenMapper(logger.Object);

            // Act
            Token token = await mapper.MapToTokenAsync("variable(myid)");

            // Assert
            Assert.IsAssignableFrom<VariableToken>(token);
        }
    }
}