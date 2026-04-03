using Faker;

namespace Faker.Generators
{
    public class ListGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public object Generate(Type type, GeneratorContext context)
        {
            Type elementType = type.GetGenericArguments()[0];

            var list = (System.Collections.IList)Activator.CreateInstance(type);

            int count = context.Random.Next(3, 10);

            for (int i = 0; i < count; i++)
            {
                object element = context.Faker.Create(elementType, context);
                list.Add(element);
            }

            return list;
        }
    }
}
