using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators.ValueGenerators
{
    public class CharGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type) => type == typeof(char);
        public object Generate(Type type, GeneratorContext context)
        {
            return (char)context.Random.Next(32, 127);
        }
    }
}
