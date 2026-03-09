using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Core
{
    public class ThreadTrace
    {
        public int ThreadId { get; }
        public long TotalTime { get; }
        public IReadOnlyList<MethodTrace> Methods { get; }

        public ThreadTrace(int threadId, long totalTime, List<MethodTrace> methods)
        {
            ThreadId = threadId;
            TotalTime = totalTime;
            Methods = methods.AsReadOnly();
        }
    }
}
