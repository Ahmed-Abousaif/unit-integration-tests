using UnitTestSample01;

namespace XunitTestProject
{
    public class CalculatorUnitTest
    {
        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(int.MaxValue, int.MinValue, -1)]
        [InlineData(-1, -1, -2)]
        [InlineData(0, 0, 0)]
        [InlineData(-100, 50, -50)]
        public void Add_Two_Integers_Will_Return_Integer(int a, int b, int expected)
        {
            // Act
            int result = Calculator.Add(a, b);
            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Add_Two_With_Max_Throw_Exception()
        {
            // Arrange
            int a = 1;
            int b = int.MaxValue;
            // Act & Assert
            Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
        }

        [Fact]
        public void Add_Two_With_Min_Value_Throw_Exception()
        {
            // Arrange
            int a = -1;
            int b = int.MinValue;
            // Act & Assert
            Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
        }
    }
}
