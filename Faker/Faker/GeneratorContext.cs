using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker
{
    public class GeneratorContext
    {
        public Faker Faker { get; }
        public Random Random { get; }

        private readonly List<Type> _typePath = new List<Type>();

        public GeneratorContext(Faker faker, Random random)
        {
            Faker = faker;
            Random = random;
        }

        public bool Enter(Type type)
        {
            if (type.IsValueType || type == typeof(string))
            {
                return true;
            }

            if (_typePath.Contains(type))
            {
                return false;
            }

            _typePath.Add(type);
            return true;
        }

        public void Leave(Type type)
        {
            if (type.IsValueType || type == typeof(string))
            {
                return;
            }

            if (_typePath.Count > 0 && _typePath[^1] == type)
            {
                _typePath.RemoveAt(_typePath.Count - 1);
            }
        }
    }
}
