using System.Linq;
using Xunit;

namespace DeveImageOptimizer.Tests
{
    public class ConstantsFileExtensionsTests
    {
        [Fact]
        public void ExtensionsOnlyContainCapitalLetters()
        {
            //Arrange
            var values = ConstantsFileExtensions.AllValidExtensions;

            //Act

            //Assert
            Assert.True(values.All(t => t.All(z => char.IsUpper(z) || z == '.')));
        }
    }
}
