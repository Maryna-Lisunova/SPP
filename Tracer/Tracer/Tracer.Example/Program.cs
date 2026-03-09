using System.Reflection;
using Tracer.Core;
using Tracer.Example;
using Tracer.Serialization;
using Tracer.Serialization.Abstractions;

internal class Program
{
    private static void Main(string[] args)
    {
        MyTracer tracer = new MyTracer();

        Console.WriteLine("\nRunning methods...");
        Foo foo = new Foo(tracer);
        foo.MyMethod();

        Thread t1 = new Thread(() => foo.MyMethod());
        Thread t2 = new Thread(() => foo.MyMethod());
        Thread t3 = new Thread(() => foo.MyMethod());

        t1.Start();
        t2.Start();
        t3.Start();

        t1.Join();
        t2.Join();
        t3.Join();

        Console.WriteLine("\nGetting trace results...");
        var traceResult = tracer.GetTraceResult();

        var traceResultDto = TraceResultConverter.Convert(traceResult);

        Console.WriteLine("\nLoading serialization plugins...");
        string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        var loader = new TraceResultSerializerLoader(pluginsPath);
        var serializers = loader.LoadSerializers();

        Console.WriteLine("\nSaving results...");
        foreach (var serializer in serializers)
        {
            string fileName = $"result.{serializer.Format}";
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            using (var fileStream = File.Create(fullPath))
            {
                serializer.Serialize(traceResultDto, fileStream);
            }
            Console.WriteLine($"   Saved: {fullPath}");
        }
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}