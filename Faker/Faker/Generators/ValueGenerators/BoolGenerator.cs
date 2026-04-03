using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators.ValueGenerators
{
    public class BoolGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type) => type == typeof(bool);
        public object Generate(Type type, GeneratorContext context) => context.Random.Next(2) == 0;
    }
}
