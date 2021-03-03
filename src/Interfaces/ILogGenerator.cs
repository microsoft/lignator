using System.Threading.Tasks;
using Lignator.Models;

namespace Lignator.Interfaces
{
    public interface ILogGenerator
    {
        Task Generate(Options options);
    }
}