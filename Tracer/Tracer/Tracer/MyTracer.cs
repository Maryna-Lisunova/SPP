using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Tracer.Core
{
    public class MyTracer : ITracer
    { // собирает информацию о методах с учетом вложенности и многопоточности
        private class MethodTraceInternal
        {
            public string MethodName;
            public string ClassName;
            public Stopwatch Stopwatch;
            public long StartTime;
            public List<MethodTraceInternal> Children = new();
        }

        private class ThreadTraceInternal
        {
            public Stack<MethodTraceInternal> ActiveTraces = new(); // стек актив методов
            public List<MethodTraceInternal> RootMethods = new(); // заверш
        }

        private readonly ConcurrentDictionary<int, ThreadTraceInternal> _threadsTracerInfo = new(); // потокобезопасный словарь

        public void StartTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            var thread = _threadsTracerInfo.GetOrAdd(threadId, (key) => new ThreadTraceInternal());
            // короткая запись - лямбда функция
            var frame = new StackTrace().GetFrame(1);
            var method = frame.GetMethod();

            var trace = new MethodTraceInternal // точка входа в вызов
            {
                MethodName = method.Name,
                ClassName = method.DeclaringType.Name,
                Stopwatch = Stopwatch.StartNew()
            };

            if (thread.ActiveTraces.Count > 0) // актив методс?
                thread.ActiveTraces.Peek().Children.Add(trace);
            else
                thread.RootMethods.Add(trace);

            thread.ActiveTraces.Push(trace);
        }

        public void StopTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_threadsTracerInfo.TryGetValue(threadId, out var thread))
                return;

            if (thread.ActiveTraces.Count == 0)
                return;

            var trace = thread.ActiveTraces.Pop();
            trace.Stopwatch.Stop();

            // если это не корневой метод, обновляем время родительского метода
            if (thread.ActiveTraces.Count > 0)
            {
                var parent = thread.ActiveTraces.Peek();
                // время родителя уже учитывается через Stopwatch
            }
        }

        public TraceResult GetTraceResult()
        {
            var result = new Dictionary<int, ThreadTrace>();

            foreach (var pair in _threadsTracerInfo)
            {
                int threadId = pair.Key;
                var internalThread = pair.Value;

                long totalTime = 0;
                var methods = new List<MethodTrace>();

                foreach (var m in internalThread.RootMethods)
                {
                    var converted = Convert(m);
                    methods.Add(converted);
                    totalTime += converted.Time; // дочернеее входит в рутовое
                }

                result[threadId] = new ThreadTrace(threadId, totalTime, methods);
            }

            return new TraceResult(result);
        }

        private MethodTrace Convert(MethodTraceInternal m)
        {
            var children = new List<MethodTrace>();
            foreach (var c in m.Children)
            {
                children.Add(Convert(c));
            }

            return new MethodTrace(m.MethodName, m.ClassName, m.Stopwatch.ElapsedMilliseconds, children);
        }
    }
}