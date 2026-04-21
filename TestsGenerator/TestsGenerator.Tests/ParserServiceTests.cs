using Xunit;
using FluentAssertions;
using TestsGenerator.Core;
using System.Linq;

namespace TestsGenerator.Tests
{
    public class ParserServiceTests
    {
        private readonly ParserService _parser = new ParserService();

        [Fact]
        public void Parse_ShouldIdentifyPublicMethods()
        {
            string sourceCode = @"
                public class Bar {
                    public void InnerMethod() { }
                    private void PrivateMethod() { }
                }";

            var result = _parser.Parse(sourceCode);

            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Bar");
            result[0].Methods.Should().ContainSingle(m => m.Name == "InnerMethod");
            result[0].Methods.Should().NotContain(m => m.Name == "PrivateMethod");
        }

        [Fact]
        public void Parse_ShouldIdentifyConstructorDependencies()
        {
            string sourceCode = @"
                public class Foo {
                    public Foo(ITracer tracer) { }
                }";

            var result = _parser.Parse(sourceCode);

            result[0].ConstructorDependencies.Should().Contain("ITracer");
        }
    }
}