using System.Collections.Generic;

namespace TestsGenerator.Core.Abstractions
{
    public interface IGenerator
    {
        IEnumerable<GeneratedFile> Generate(string sourceCode);
    }
}