namespace UnitTestSample01
{
    public static class Calculator
    {
        public static int Add(int a, int b)
        {
            // Check for positive overflow
            if (b > 0 && a > int.MaxValue - b)
            {
                throw new OverflowException("Addition results in an overflow.");
            }

            // Check for negative overflow (underflow)
            if (b < 0 && a < int.MinValue - b)
            {
                throw new OverflowException("Addition results in an underflow.");
            }

            return a + b;
        }
    }
}