using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators
{
    public class StringGenerator : IValueGenerator
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public bool CanGenerate(Type type)
        {
            return type == typeof(string);
        }

        public object Generate(Type type, GeneratorContext context)
        {
            int length = context.Random.Next(5, 15);
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = context.Random.Next(Alphabet.Length);
                sb.Append(Alphabet[index]);
            }

            return sb.ToString();
        }
    }
}
