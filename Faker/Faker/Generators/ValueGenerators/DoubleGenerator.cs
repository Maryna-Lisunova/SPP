using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators.ValueGenerators
{
    public class DoubleGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type) => type == typeof(double);
        public object Generate(Type type, GeneratorContext context) => context.Random.NextDouble() * 1000;
    }
}
