using System;
using NUnit.Framework;
using Moq;

namespace Tests
{
    [TestFixture]
    public class BarTests
    {
        private Bar _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new Bar();
        }

        [Test]
        public void InnerMethod_Test()
        {
            // Arrange

            // Act
            _testClass.InnerMethod();

            // Assert
        }
    }
}
