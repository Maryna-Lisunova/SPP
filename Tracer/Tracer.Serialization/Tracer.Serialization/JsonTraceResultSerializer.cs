using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Json
{
    public class JsonTraceResultSerializer : ITraceResultSerializer
    {
        public string Format => "json";

        public void Serialize(TraceResultDto traceResult, Stream to)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            JsonSerializer.Serialize(to, traceResult, options);
        }
    }
}