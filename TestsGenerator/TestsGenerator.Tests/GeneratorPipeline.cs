using Xunit;
using TestsGenerator.Core;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestsGenerator.Tests
{
    public class PipelineTests
    {
        [Fact]
        public async Task Pipeline_ShouldProcessMultipleFilesParallel()
        {
            string inputDir = Path.Combine(Path.GetTempPath(), "InputFiles");
            string outputDir = Path.Combine(Path.GetTempPath(), "OutputFiles");
            Directory.CreateDirectory(inputDir);
            Directory.CreateDirectory(outputDir);

            var files = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string path = Path.Combine(inputDir, $"Class{i}.cs");
                File.WriteAllText(path, $"public class Class{i} {{ public void M() {{}} }}");
                files.Add(path);
            }

            var generator = new CodeGenerator(new ParserService());
            var pipeline = new GeneratorPipeline(generator);

            await pipeline.RunAsync(files, outputDir, 2, 2, 2);

            for (int i = 0; i < 5; i++)
            {
                Assert.True(File.Exists(Path.Combine(outputDir, $"Class{i}Tests.cs")));
            }

            Directory.Delete(inputDir, true);
            Directory.Delete(outputDir, true);
        }
    }
}