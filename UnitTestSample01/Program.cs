using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using UnitTestSample01.Entities;
using UnitTestSample01.Model;
using UnitTestSample01.Services;

namespace UnitTestSample01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=UnitTestSample01Db;Trusted_Connection=True;MultipleActiveResultSets=true");
            AppDbContext appDbContext = new AppDbContext(optionsBuilder.Options);
            Console.WriteLine("Departments:");

            //DepartmentsService departmentsService = new DepartmentsService(appDbContext);
            //Department department = new Department();
            //departmentsService.AddDepartment(department);

            //Console.WriteLine("Hello, World!");
            //int x = 1;
            //int y = 2;
            //int result = Calculator.Add(x, y);
            //Console.WriteLine($"{x} + {y} = {result}");
            //x = int.MaxValue;
            //y = 1;

            //result = Calculator.Add(x, y);
            //Console.WriteLine($"{x} + {y} = {result}");

            //x = int.MinValue;
            //y = -1;
            //result = Calculator.Add(x, y);
            //Console.WriteLine($"{x} + {y} = {result}");
            Console.ReadKey();
        }
    }

}
