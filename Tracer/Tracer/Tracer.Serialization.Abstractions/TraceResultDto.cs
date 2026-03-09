using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Serialization.Abstractions
{
    public class TraceResultDto
    {
        public List<ThreadTraceDto> Threads { get; set; } = new();
    }

    public class ThreadTraceDto
    {
        public int Id { get; set; }
        public string Time { get; set; } = string.Empty;
        public List<MethodTraceDto> Methods { get; set; } = new();
    }

    public class MethodTraceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public List<MethodTraceDto> Methods { get; set; } = new();
    }
}