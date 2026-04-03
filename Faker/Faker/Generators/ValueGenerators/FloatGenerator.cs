using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators.ValueGenerators
{
    public class FloatGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type) => type == typeof(float);
        public object Generate(Type type, GeneratorContext context) => (float)context.Random.NextDouble() * 1000;
    }
}
