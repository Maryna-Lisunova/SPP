using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators.ValueGenerators
{
    public class ByteGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type) => type == typeof(byte);
        public object Generate(Type type, GeneratorContext context) => (byte)context.Random.Next(0, 256);
    }
}
