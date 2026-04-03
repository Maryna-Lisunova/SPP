using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators
{
    public class ArrayGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type.IsArray;
        }

        public object Generate(Type type, GeneratorContext context)
        {
            Type elementType = type.GetElementType();
            int length = context.Random.Next(3, 10);
            Array array = Array.CreateInstance(elementType, length);

            for (int i = 0; i < length; i++)
            {
                object element = context.Faker.Create(elementType, context);
                array.SetValue(element, i);
            }
            return array;
        }
    }
}
