namespace UnitTestSample01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            int x = 1;
            int y = 2;
            int result = Calculator.Add(x, y);
            Console.WriteLine($"{x} + {y} = {result}");
            x = int.MaxValue;
            y = 1;

            result = Calculator.Add(x, y);
            Console.WriteLine($"{x} + {y} = {result}");

            x = int.MinValue;
            y = -1;
            result = Calculator.Add(x, y);
            Console.WriteLine($"{x} + {y} = {result}");
            Console.ReadKey();
        }
    }

}
