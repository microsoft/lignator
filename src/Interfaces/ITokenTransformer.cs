using System;
using System.Collections.Generic;
using Lignator.Models;

namespace Lignator.Interfaces
{
    public interface ITokenTransformer
    {
        string Transform(string template, IEnumerable<Token> tokens, Random random, IDictionary<string, Extraction> variables);
    }
}