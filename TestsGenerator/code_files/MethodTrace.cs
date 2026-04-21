using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Core
{
    public class MethodTrace 
    {
        public string MethodName { get; }
        public string ClassName { get; }
        public long Time { get; }
        public IReadOnlyList<MethodTrace> Children { get; }

        public MethodTrace(string methodName, string className, long time, List<MethodTrace> children)
        {
            MethodName = methodName;
            ClassName = className;
            Time = time;
            Children = children.AsReadOnly();
        }
    }
}
