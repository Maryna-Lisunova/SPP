using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization
{
    public class TraceResultSerializerLoader
    {
        private readonly string _pluginsPath;

        public TraceResultSerializerLoader(string pluginsPath)
        {
            _pluginsPath = pluginsPath;
        }

        public IEnumerable<ITraceResultSerializer> LoadSerializers()
        {
            var serializers = new List<ITraceResultSerializer>();

            if (!Directory.Exists(_pluginsPath))
            {
                return serializers;
            }

            foreach (var dllPath in Directory.GetFiles(_pluginsPath, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dllPath);

                    var serializerTypes = assembly.GetTypes()
                        .Where(t => typeof(ITraceResultSerializer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in serializerTypes)
                    {
                        if (Activator.CreateInstance(type) is ITraceResultSerializer serializer)
                        {
                            serializers.Add(serializer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading {dllPath}: {ex.Message}");
                }
            }
            return serializers;
        }
    }
}