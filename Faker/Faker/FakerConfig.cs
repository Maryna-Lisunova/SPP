using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Faker
{
    public class FakerConfig
    {
        private readonly Dictionary<(Type, string), IValueGenerator> _customGenerators = new();

        public void Add<TTarget, TMember, TGenerator>(Expression<Func<TTarget, TMember>> expression)
            where TGenerator : IValueGenerator, new()
        {
            if (expression.Body is not MemberExpression memberExpression)
            {
                if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression innerMember)
                {
                    memberExpression = innerMember;
                }
                else
                {
                    throw new ArgumentException("Expression must be a member access (property or field).");
                }
            }

            string memberName = memberExpression.Member.Name;
            var generator = new TGenerator();
            _customGenerators[(typeof(TTarget), memberName)] = generator;
        }

        public bool TryGetGenerator(Type targetType, string memberName, out IValueGenerator generator)
        {
            return _customGenerators.TryGetValue((targetType, memberName), out generator);
        }
    }
}
