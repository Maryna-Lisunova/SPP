using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace Faker
{
    using global::Faker.Generators.ValueGenerators;
    using global::Faker.Generators;

    public class Faker
    {
        private readonly List<IValueGenerator> _generators = new List<IValueGenerator>();
        private readonly FakerConfig _config;
        private readonly Random _random = new Random();

        public Faker(FakerConfig config = null)
        {
            _config = config ?? new FakerConfig();

            _generators.Add(new BoolGenerator());
            _generators.Add(new ByteGenerator());
            _generators.Add(new CharGenerator());
            _generators.Add(new DoubleGenerator());
            _generators.Add(new FloatGenerator());
            _generators.Add(new IntGenerator());
            _generators.Add(new LongGenerator());

            _generators.Add(new ArrayGenerator());
            _generators.Add(new DateTimeGenerator());
            _generators.Add(new ListGenerator());
            _generators.Add(new StringGenerator());
        }

        public T Create<T>()
        {
            var context = new GeneratorContext(this, _random);
            return (T)Create(typeof(T), context);
        }

        internal object Create(Type type, GeneratorContext context)
        {
            if (!context.Enter(type))
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            try
            {
                var generator = _generators.FirstOrDefault(g => g.CanGenerate(type));
                if (generator != null)
                {
                    return generator.Generate(type, context);
                }

                return CreateComplexObject(type, context);
            }
            finally
            {
                context.Leave(type);
            }
        }

        private object CreateComplexObject(Type type, GeneratorContext context)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderByDescending(c => c.GetParameters().Length)
                .ToList();

            object instance = null;
            ParameterInfo[] usedParameters = null;

            foreach (var ctor in constructors)
            {
                try
                {
                    var parameters = ctor.GetParameters();
                    var args = new object[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (_config.TryGetGenerator(type, parameters[i].Name, out var customGen))
                        {
                            args[i] = customGen.Generate(parameters[i].ParameterType, context);
                        }
                        else
                        {
                            args[i] = Create(parameters[i].ParameterType, context);
                        }
                    }

                    instance = ctor.Invoke(args);
                    usedParameters = parameters;
                    break;
                }
                catch
                {
                    continue;
                }
            }

            if (instance == null && type.IsValueType)
            {
                instance = Activator.CreateInstance(type);
            }

            if (instance != null)
            {
                FillPropertiesAndFields(instance, type, context, usedParameters);
            }

            return instance;
        }

        private void FillPropertiesAndFields(object instance, Type type, GeneratorContext context, ParameterInfo[] usedParameters)
        {
            if (instance == null) return;

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetSetMethod() != null)
                .Select(p => new { Member = (MemberInfo)p, p.PropertyType, SetValue = (Action<object, object>)p.SetValue });

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Select(f => new { Member = (MemberInfo)f, PropertyType = f.FieldType, SetValue = (Action<object, object>)f.SetValue });

            var members = properties.Concat(fields);

            foreach (var member in members)
            {
                if (IsAlreadyInitialized(member.Member.Name, member.PropertyType, usedParameters))
                    continue;

                object value;
                if (_config.TryGetGenerator(type, member.Member.Name, out var customGen))
                {
                    value = customGen.Generate(member.PropertyType, context);
                }
                else
                {
                    value = context.Faker.Create(member.PropertyType, context);
                }
                member.SetValue(instance, value);
            }
        }

        private bool IsAlreadyInitialized(string memberName, Type memberType, ParameterInfo[] usedParameters)
        {
            if (usedParameters == null) return false;

            return usedParameters.Any(p => p.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase) &&
                p.ParameterType == memberType);
        }
    }
}
