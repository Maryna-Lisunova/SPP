using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators.ValueGenerators
{
    public class LongGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type) => type == typeof(long);
        public object Generate(Type type, GeneratorContext context)
        {
            return context.Random.NextInt64();
        }
    }
}
