using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TestsGenerator.Core
{
    public record GeneratedFile(string FileName, string Content);

    public class GeneratorPipeline
    {
        private readonly IGenerator _generator;

        public GeneratorPipeline(IGenerator generator)
        {
            _generator = generator;
        }

        public async Task RunAsync(IEnumerable<string> inputFiles, string outputPath, int maxLoadDegree, int maxGenDegree, int maxSaveDegree)
        {
            var loadBlock = new TransformBlock<string, string>(async path => await File.ReadAllTextAsync(path),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxLoadDegree });

            var generateBlock = new TransformManyBlock<string, GeneratedFile>(content => _generator.Generate(content),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxGenDegree });

            var saveBlock = new ActionBlock<GeneratedFile>(
                async item =>
                {
                    var fullPath = Path.Combine(outputPath, item.FileName);
                    await File.WriteAllTextAsync(fullPath, item.Content);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxSaveDegree });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            loadBlock.LinkTo(generateBlock, linkOptions);
            generateBlock.LinkTo(saveBlock, linkOptions);

            foreach (var file in inputFiles)
            {
                await loadBlock.SendAsync(file);
            }

            loadBlock.Complete();
            await saveBlock.Completion;
        }
    }

    public interface IGenerator
    {
        IEnumerable<GeneratedFile> Generate(string sourceCode);
    }
}