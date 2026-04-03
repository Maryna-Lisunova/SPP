using Xunit;
using System;
using System.Collections.Generic;
using Faker;

namespace TestsForFaker
{
    #region Test Models

    public class Dog { public string Name { get; set; } }

    public class A { public B B { get; set; } }
    public class B { public C C { get; set; } }
    public class C { public A A { get; set; } }

    public class Address { public string City { get; set; } }
    public class Person
    {
        public Address Home { get; set; }
        public Address Office { get; set; }
    }

    public class PrivateCtorClass
    {
        public string Secret { get; private set; }
        private PrivateCtorClass(string secret) => Secret = secret;
    }

    public class FallbackCtorClass
    {
        public string Value { get; }
        public FallbackCtorClass(string a, int b, double c) => throw new Exception("Boom!");
        public FallbackCtorClass(string value) => Value = value;
    }

    public struct CustomStruct
    {
        public int X;
        public string Name { get; set; }
    }

    public class FieldClass { public string PublicField; }

    public class ConstantNameGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t) => t == typeof(string);
        public object Generate(Type t, GeneratorContext c) => "Rex";
    }

    #endregion

    public class FakerTests
    {
        private readonly Faker.Faker _faker = new();

        [Fact]
        public void Create_PrimitiveTypes_WorkCorrectly()
        {
            Assert.NotEqual(0, _faker.Create<int>());
            Assert.NotEqual(0, _faker.Create<long>());
            Assert.True(_faker.Create<double>() >= 0);
        }

        [Fact]
        public void Create_ListWithComplexObjects_ShouldPopulateCorrectly()
        {
            var dogs = _faker.Create<List<Dog>>();

            Assert.NotNull(dogs);
            Assert.NotEmpty(dogs);
            Assert.All(dogs, dog => Assert.False(string.IsNullOrEmpty(dog.Name)));
        }

        [Fact]
        public void Create_NestedCollections_ShouldBePopulated()
        {
            var result = _faker.Create<List<int[]>>();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, array => {
                Assert.NotNull(array);
                Assert.NotEmpty(array);
            });
        }

        [Fact]
        public void Create_PrivateConstructor_ShouldBeInvoked()
        {
            var result = _faker.Create<PrivateCtorClass>();
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.Secret));
        }

        [Fact]
        public void Create_ConstructorThrows_ShouldFallbackToNextBest()
        {
            var result = _faker.Create<FallbackCtorClass>();
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.Value));
        }

        [Fact]
        public void Create_CustomStruct_ShouldBePopulated()
        {
            var result = _faker.Create<CustomStruct>();
            Assert.NotEqual(0, result.X);
            Assert.False(string.IsNullOrEmpty(result.Name));
        }

        [Fact]
        public void Create_RecursiveCircularDependency_ShouldBreakCycleWithNull()
        {
            var a = _faker.Create<A>();

            Assert.NotNull(a?.B?.C);
            Assert.Null(a.B.C.A); 
        }

        [Fact]
        public void Create_DuplicateTypesInDifferentBranches_ShouldNotBeNull()
        {
            var person = _faker.Create<Person>();

            Assert.NotNull(person.Home);
            Assert.NotNull(person.Office);
            Assert.NotSame(person.Home, person.Office);
        }

        [Fact]
        public void Config_CustomGenerator_ShouldBeUsedForProperties()
        {
            var config = new FakerConfig();
            config.Add<Dog, string, ConstantNameGenerator>(d => d.Name);
            var fakerWithConfig = new Faker.Faker(config);

            var dog = fakerWithConfig.Create<Dog>();

            Assert.Equal("Rex", dog.Name);
        }

        [Fact]
        public void Config_CustomGenerator_ShouldWorkForPublicFields()
        {
            var config = new FakerConfig();
            config.Add<FieldClass, string, ConstantNameGenerator>(f => f.PublicField);
            var fakerWithConfig = new Faker.Faker(config);

            var result = fakerWithConfig.Create<FieldClass>();

            Assert.Equal("Rex", result.PublicField);
        }
    }
}