using DeveImageOptimizer.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DeveImageOptimizer.Tests.Helpers
{    
    public class ValuesToStringHelperTests
    {
        [Fact]
        public void ConvertsBytesToMb()
        {
            //Arrange

            //Act
            var result = ValuesToStringHelper.BytesToString(12345678L);

            //Assert
            Assert.Equal("11,8MB", result);
        }

        [Fact]
        public void ConvertsBytesToKb()
        {
            //Arrange

            //Act
            var result = ValuesToStringHelper.BytesToString(1024L);

            //Assert
            Assert.Equal("1KB", result);
        }

        [Fact]
        public void Converts0BytesToB()
        {
            //Arrange

            //Act
            var result = ValuesToStringHelper.BytesToString(0L);

            //Assert
            Assert.Equal("0B", result);
        }


        [Fact]
        public void ConvertsSecondsToHours()
        {
            //Arrange

            //Act
            var result = ValuesToStringHelper.SecondsToString(3800L);

            //Assert
            Assert.Equal("1,1 Hours", result);
        }

        [Fact]
        public void ConvertsSecondsToHours2()
        {
            //Arrange

            //Act
            var result = ValuesToStringHelper.SecondsToString(3600L);

            //Assert
            Assert.Equal("1 Hour", result);
        }

        [Fact]
        public void ConvertsSecondsToMinutes()
        {
            //Arrange

            //Act
            var result = ValuesToStringHelper.SecondsToString(120L);

            //Assert
            Assert.Equal("2 Minutes", result);
        }

        [Fact]
        public void Converts0SecondsToSeconds()
        {
            //Arrange

            //Act
            var result = ValuesToStringHelper.SecondsToString(0L);

            //Assert
            Assert.Equal("0 Seconds", result);
        }
    }
}
