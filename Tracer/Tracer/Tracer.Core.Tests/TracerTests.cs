using Tracer.Core;

namespace Tracer.Core.Tests
{
    public class TracerTests
    {
        [Test]
        public void StartTrace_ShouldRecordMethodInfo()
        {
            var tracer = new MyTracer();

            tracer.StartTrace();
            Thread.Sleep(50);
            tracer.StopTrace();

            var result = tracer.GetTraceResult();
            Assert.That(result.Threads.Count, Is.EqualTo(1));
            Assert.That(result.Threads[Thread.CurrentThread.ManagedThreadId].Methods.Count, Is.EqualTo(1));

            var method = result.Threads[Thread.CurrentThread.ManagedThreadId].Methods[0];
            Assert.That(method.MethodName, Is.EqualTo(nameof(StartTrace_ShouldRecordMethodInfo)));
            Assert.That(method.ClassName, Is.EqualTo(nameof(TracerTests)));
            Assert.That(method.Time, Is.GreaterThanOrEqualTo(50));
        }

        [Test]
        public void NestedMethods_ShouldBeRecordedCorrectly()
        { // вложенные 
            var tracer = new MyTracer();

            tracer.StartTrace();
            Thread.Sleep(50);
            InnerMethod(tracer);
            Thread.Sleep(30);
            tracer.StopTrace();

            var result = tracer.GetTraceResult();
            var threadResult = result.Threads[Thread.CurrentThread.ManagedThreadId];

            Assert.That(threadResult.Methods.Count, Is.EqualTo(1));

            var outerMethod = threadResult.Methods[0];
            Assert.That(outerMethod.MethodName, Is.EqualTo(nameof(NestedMethods_ShouldBeRecordedCorrectly)));
            Assert.That(outerMethod.Children.Count, Is.EqualTo(1));

            var innerMethod = outerMethod.Children[0];
            Assert.That(innerMethod.MethodName, Is.EqualTo(nameof(InnerMethod)));
            Assert.That(outerMethod.Time, Is.GreaterThanOrEqualTo(innerMethod.Time));
        }

        [Test]
        public void MultipleThreads_ShouldBeRecordedSeparately()
        {
            var tracer = new MyTracer();
            var threads = new List<Thread>();
            const int threadCount = 3;

            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(() =>
                {
                    tracer.StartTrace();
                    Thread.Sleep(30);
                    tracer.StopTrace();
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            var result = tracer.GetTraceResult();
            // главный поток не вызывал StartTrace, поэтому должно быть только threadCount потоков
            Assert.That(result.Threads.Count, Is.EqualTo(threadCount));
        }

        [Test]
        public void MultipleRootMethods_ShouldBeRecordedAtSameLevel()
        {
            var tracer = new MyTracer();

            Method1(tracer);
            Method2(tracer);

            var result = tracer.GetTraceResult();
            var threadResult = result.Threads[Thread.CurrentThread.ManagedThreadId];

            Assert.That(threadResult.Methods.Count, Is.EqualTo(2));
            Assert.That(threadResult.Methods[0].MethodName, Is.EqualTo(nameof(Method1)));
            Assert.That(threadResult.Methods[1].MethodName, Is.EqualTo(nameof(Method2)));
        }

        [Test]
        public void ThreadTime_ShouldBeSumOfRootMethods()
        {
            var tracer = new MyTracer();

            tracer.StartTrace();
            Thread.Sleep(100);
            tracer.StopTrace();

            tracer.StartTrace();
            Thread.Sleep(200);
            tracer.StopTrace();

            var result = tracer.GetTraceResult();
            var threadResult = result.Threads[Thread.CurrentThread.ManagedThreadId];

            long expectedSum = threadResult.Methods[0].Time + threadResult.Methods[1].Time;
            Assert.That(threadResult.TotalTime, Is.EqualTo(expectedSum));
            Assert.That(threadResult.TotalTime, Is.GreaterThanOrEqualTo(300));
        }

        [Test]
        public void EmptyTrace_ShouldReturnEmptyResult()
        {
            var tracer = new MyTracer();

            var result = tracer.GetTraceResult();

            Assert.That(result.Threads.Count, Is.EqualTo(0));
        }

        [Test]
        public void StopTrace_WithoutStart_ShouldNotThrow()
        {
            var tracer = new MyTracer();
            Assert.DoesNotThrow(() => tracer.StopTrace());
        }

        [Test]
        public void TraceResult_ShouldBeImmutable()
        {
            var tracer = new MyTracer();
            tracer.StartTrace();
            tracer.StopTrace();

            var result = tracer.GetTraceResult();

            Assert.That(result.Threads, Is.InstanceOf<IReadOnlyDictionary<int, ThreadTrace>>());

            var thread = result.Threads.First().Value;
            Assert.That(thread.Methods, Is.InstanceOf<IReadOnlyList<MethodTrace>>());

            var method = thread.Methods[0];
            Assert.That(method.Children, Is.InstanceOf<IReadOnlyList<MethodTrace>>());
        }

        private static void InnerMethod(ITracer tracer)
        {
            tracer.StartTrace();
            Thread.Sleep(20);
            tracer.StopTrace();
        }

        private static void Method1(ITracer tracer)
        {
            tracer.StartTrace();
            Thread.Sleep(30);
            tracer.StopTrace();
        }

        private static void Method2(ITracer tracer)
        {
            tracer.StartTrace();
            Thread.Sleep(40);
            tracer.StopTrace();
        }
    }
}