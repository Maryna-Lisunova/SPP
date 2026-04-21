using System.Collections.Generic;
using System.Threading.Tasks;
using TestsGenerator.Core;

namespace TestsGenerator.Console
{
    public class PipelineRunner
    {
        private readonly IGenerator _generator;

        public PipelineRunner(IGenerator generator)
        {
            _generator = generator;
        }

        public async Task RunAsync(IEnumerable<string> inputFiles, string outputPath)
        {
            var pipeline = new GeneratorPipeline(_generator);

            await pipeline.RunAsync(inputFiles, outputPath, maxLoadDegree:2, maxGenDegree:4, maxSaveDegree:2);
        }
    }
}