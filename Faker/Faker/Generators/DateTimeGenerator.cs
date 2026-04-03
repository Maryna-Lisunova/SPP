using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Generators
{
    public class DateTimeGenerator : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(DateTime);
        }

        public object Generate(Type type, GeneratorContext context)
        {
            DateTime start = DateTime.Now.AddYears(-10);
            int range = (DateTime.Today - start).Days;

            return start.AddDays(context.Random.Next(range))
                        .AddHours(context.Random.Next(0, 24))
                        .AddMinutes(context.Random.Next(0, 60))
                        .AddSeconds(context.Random.Next(0, 60));
        }
    }
}
