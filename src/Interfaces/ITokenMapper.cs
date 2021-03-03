using System.Threading.Tasks;
using Lignator.Models;

namespace Lignator.Interfaces
{
    public interface ITokenMapper
    {
        Task<Token> MapToTokenAsync(string rawToken);
    }
}