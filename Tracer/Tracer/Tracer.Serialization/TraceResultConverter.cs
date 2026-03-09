using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization
{
    public static class TraceResultConverter
    {
        public static TraceResultDto Convert(TraceResult traceResult)
        {
            var dto = new TraceResultDto();

            foreach (var thread in traceResult.Threads.Values)
            {
                dto.Threads.Add(new ThreadTraceDto
                {
                    Id = thread.ThreadId,
                    Time = $"{thread.TotalTime}ms",
                    Methods = ConvertMethods(thread.Methods)
                });
            }
            return dto;
        }

        private static List<MethodTraceDto> ConvertMethods(IReadOnlyList<MethodTrace> methods)
        {
            var result = new List<MethodTraceDto>();

            foreach (var method in methods)
            {
                result.Add(new MethodTraceDto
                {
                    Name = method.MethodName,
                    Class = method.ClassName,
                    Time = $"{method.Time}ms",
                    Methods = ConvertMethods(method.Children)
                });
            }
            return result;
        }
    }
}