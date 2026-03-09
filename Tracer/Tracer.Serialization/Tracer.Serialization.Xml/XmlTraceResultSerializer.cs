using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Xml
{
    public class XmlTraceResultSerializer : ITraceResultSerializer
    {
        public string Format => "xml";

        public void Serialize(TraceResultDto traceResult, Stream to)
        {
            var serializer = new XmlSerializer(typeof(TraceResultDto));
            serializer.Serialize(to, traceResult);
        }
    }
}