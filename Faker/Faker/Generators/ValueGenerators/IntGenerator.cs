using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators.ValueGenerators
{
    public class IntGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(int);
        }

        public object Generate(Type type, GeneratorContext context)
        {
            return context.Random.Next();
        }
    }
}
