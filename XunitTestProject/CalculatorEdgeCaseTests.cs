using UnitTestSample01;

namespace XunitTestProject
{
    /// <summary>
    /// Comprehensive edge case tests for Calculator.Add method
    /// Focuses on overflow/underflow validation and boundary conditions
    /// </summary>
    public class CalculatorEdgeCaseTests
    {
        #region Overflow Tests - Positive Numbers
        
        [Fact]
        public void Add_MaxValuePlusOne_ShouldThrowOverflowException()
        {
            // Arrange
            int a = int.MaxValue;
            int b = 1;
            
            // Act & Assert
            var exception = Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
            Assert.Contains("overflow", exception.Message.ToLower());
        }

        [Fact]
        public void Add_MaxValuePlusMaxValue_ShouldThrowOverflowException()
        {
            // Arrange
            int a = int.MaxValue;
            int b = int.MaxValue;
            
            // Act & Assert
            var exception = Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
            Assert.Contains("overflow", exception.Message.ToLower());
        }

        [Fact]
        public void Add_LargePositiveNumbers_ShouldThrowOverflowException()
        {
            // Arrange
            int a = int.MaxValue - 100;
            int b = 200;
            
            // Act & Assert
            Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
        }

        #endregion

        #region Underflow Tests - Negative Numbers

        [Fact]
        public void Add_MinValueMinusOne_ShouldThrowOverflowException()
        {
            // Arrange
            int a = int.MinValue;
            int b = -1;
            
            // Act & Assert
            var exception = Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
            Assert.Contains("underflow", exception.Message.ToLower());
        }

        [Fact]
        public void Add_MinValuePlusMinValue_ShouldThrowOverflowException()
        {
            // Arrange
            int a = int.MinValue;
            int b = int.MinValue;
            
            // Act & Assert
            var exception = Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
            Assert.Contains("underflow", exception.Message.ToLower());
        }

        [Fact]
        public void Add_LargeNegativeNumbers_ShouldThrowOverflowException()
        {
            // Arrange
            int a = int.MinValue + 100;
            int b = -200;
            
            // Act & Assert
            Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
        }

        #endregion

        #region Boundary Safe Cases

        [Fact]
        public void Add_MaxValuePlusZero_ShouldReturnMaxValue()
        {
            // Arrange
            int a = int.MaxValue;
            int b = 0;
            
            // Act
            int result = Calculator.Add(a, b);
            
            // Assert
            Assert.Equal(int.MaxValue, result);
        }

        [Fact]
        public void Add_MinValuePlusZero_ShouldReturnMinValue()
        {
            // Arrange
            int a = int.MinValue;
            int b = 0;
            
            // Act
            int result = Calculator.Add(a, b);
            
            // Assert
            Assert.Equal(int.MinValue, result);
        }

        [Fact]
        public void Add_MaxValuePlusMinValue_ShouldReturnNegativeOne()
        {
            // Arrange
            int a = int.MaxValue;
            int b = int.MinValue;
            
            // Act
            int result = Calculator.Add(a, b);
            
            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void Add_MaxValueMinusOne_ShouldReturnMaxValueMinusOne()
        {
            // Arrange
            int a = int.MaxValue;
            int b = -1;
            
            // Act
            int result = Calculator.Add(a, b);
            
            // Assert
            Assert.Equal(int.MaxValue - 1, result);
        }

        [Fact]
        public void Add_MinValuePlusOne_ShouldReturnMinValuePlusOne()
        {
            // Arrange
            int a = int.MinValue;
            int b = 1;
            
            // Act
            int result = Calculator.Add(a, b);
            
            // Assert
            Assert.Equal(int.MinValue + 1, result);
        }

        #endregion

        #region Parameter Order Tests

        [Fact]
        public void Add_PositiveOverflow_OrderShouldNotMatter()
        {
            // Test that Add(a, b) and Add(b, a) both throw for overflow
            // Arrange
            int a = 1;
            int b = int.MaxValue;
            
            // Act & Assert
            Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
            Assert.Throws<OverflowException>(() => Calculator.Add(b, a));
        }

        [Fact]
        public void Add_NegativeOverflow_OrderShouldNotMatter()
        {
            // Test that Add(a, b) and Add(b, a) both throw for underflow
            // Arrange
            int a = -1;
            int b = int.MinValue;
            
            // Act & Assert
            Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
            Assert.Throws<OverflowException>(() => Calculator.Add(b, a));
        }

        #endregion

        #region Edge Boundary Tests

        [Theory]
        [InlineData(int.MaxValue - 1, 1, int.MaxValue)] // Just at the boundary
        [InlineData(int.MinValue + 1, -1, int.MinValue)] // Just at the boundary
        [InlineData(1000000, -500000, 500000)] // Large numbers within safe range
        [InlineData(-1000000, 500000, -500000)] // Large mixed numbers within safe range
        public void Add_BoundaryValues_ShouldReturnCorrectResult(int a, int b, int expected)
        {
            // Act
            int result = Calculator.Add(a, b);
            
            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(int.MaxValue - 1, 2)] // Just over the boundary
        [InlineData(int.MinValue + 1, -2)] // Just under the boundary
        [InlineData(int.MaxValue / 2, int.MaxValue / 2 + 2)] // Large positive overflow
        [InlineData(int.MinValue / 2, int.MinValue / 2 - 1)] // Large negative overflow
        public void Add_JustOverBoundary_ShouldThrowOverflowException(int a, int b)
        {
            // Act & Assert
            Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
        }

        #endregion

        #region Zero and Identity Tests

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(42, 0, 42)]
        [InlineData(0, -42, -42)]
        [InlineData(-100, 0, -100)]
        public void Add_WithZero_ShouldReturnOtherOperand(int a, int b, int expected)
        {
            // Act
            int result = Calculator.Add(a, b);
            
            // Assert
            Assert.Equal(expected, result);
        }

        #endregion
    }
}