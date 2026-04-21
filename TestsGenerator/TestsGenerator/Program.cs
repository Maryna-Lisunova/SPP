using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestsGenerator.Core;

namespace TestsGenerator.ConsoleApp
{
    using System;
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Ошибка: Недостаточно аргументов.");
                Console.WriteLine("Использование: <output_path> <file1.cs> <file2.cs> ...");
                return;
            }

            string outputPath = args[0];
            var inputFiles = args.Skip(1).ToList();

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var parser = new ParserService();
            var generator = new CodeGenerator(parser);
            var pipeline = new GeneratorPipeline(generator);

            Console.WriteLine($"Начинаю генерацию для {inputFiles.Count} файлов...");
            Console.WriteLine($"Результаты будут сохранены в: {outputPath}");

            try
            {
                await pipeline.RunAsync(inputFiles, outputPath, maxLoadDegree: 2, maxGenDegree: 4, maxSaveDegree: 2);
                Console.WriteLine("Генерация успешно завершена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла критическая ошибка: {ex.Message}");
            }
        }
    }
}