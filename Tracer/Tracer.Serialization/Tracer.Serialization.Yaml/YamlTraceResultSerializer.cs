using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tracer.Serialization.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tracer.Serialization.Yaml
{
    public class YamlTraceResultSerializer : ITraceResultSerializer
    {
        public string Format => "yaml";

        public void Serialize(TraceResultDto traceResult, Stream to)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithIndentedSequences()
                .Build();

            using (var writer = new StreamWriter(to))
            {
                serializer.Serialize(writer, traceResult);
            }
        }
    }
}