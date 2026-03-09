using System.IO;

namespace Tracer.Serialization.Abstractions
{
    public interface ITraceResultSerializer
    {
        string Format { get; }
        void Serialize(TraceResultDto traceResult, Stream to);
    }
}