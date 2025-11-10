using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestSample01.Entities;
using UnitTestSample01.Model;

namespace UnitTestSample01.Services
{
    public class DepartmentsService
    {
        AppDbContext _dbContext;
        private readonly ILogger<DepartmentsService> _logger;

        public DepartmentsService(AppDbContext appDbContext
            , ILogger<DepartmentsService> logger)
        {
            _dbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _logger = logger;
        }

        // Add method to add department
        public void AddDepartment(Department department)
        {
            ArgumentNullException.ThrowIfNull(department);
            // validate department properties

            if (string.IsNullOrWhiteSpace(department.Name))
            {
                throw new ArgumentException("Department name cannot be null or empty.", nameof(department.Name));
            }

            if (department.Name.Length > 100)
            {
                throw new ArgumentException("Department name cannot exceed 100 characters.", nameof(department.Name));
            }

            if (!string.IsNullOrWhiteSpace(department.Description) && department.Description.Length > 500)
            {
                throw new ArgumentException("Department description cannot exceed 500 characters.", nameof(department.Description));
            }

            // Add validation for duplicate department names
            bool isDuplicate = _dbContext.Departments.Any(d => d.Name == department.Name);
            if (isDuplicate)
            {
                throw new InvalidOperationException($"A department with the name '{department.Name}' already exists.");
            }

            _dbContext.Departments.Add(department);
            _dbContext.SaveChanges();
        }

        // Add Update method to update department with the same validations
        public void UpdateDepartment(Department department)
        {
            ArgumentNullException.ThrowIfNull(department);
            // validate department properties
            if (string.IsNullOrWhiteSpace(department.Name))
            {
                throw new ArgumentException("Department name cannot be null or empty.", nameof(department.Name));
            }
            if (department.Name.Length > 100)
            {
                throw new ArgumentException("Department name cannot exceed 100 characters.", nameof(department.Name));
            }
            if (!string.IsNullOrWhiteSpace(department.Description) && department.Description.Length > 500)
            {
                throw new ArgumentException("Department description cannot exceed 500 characters.", nameof(department.Description));
            }
            // Add validation for duplicate department names
            bool isDuplicate = _dbContext.Departments.Any(d => d.Name == department.Name && d.Id != department.Id);
            if (isDuplicate)
            {
                throw new InvalidOperationException($"A department with the name '{department.Name}' already exists.");
            }
            _dbContext.Departments.Update(department);
            _dbContext.SaveChanges();
        }

        public void DeleteDepartment(Department department)
        {
            ArgumentNullException.ThrowIfNull(department);
            _dbContext.Departments.Remove(department);
            _dbContext.SaveChanges();
        }

        public List<Department> GetAllDepartments()
        {
            return _dbContext.Departments.ToList();
        }

        public Department GetDepartmentById(int id)
        {
            return _dbContext.Departments.FirstOrDefault(d => d.Id == id);
        }

        // Add method to get department by name
        public Department GetDepartmentByName(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            return _dbContext.Departments.FirstOrDefault(d => d.Name == name);
        }

        // search by name containing keyword
        public List<Department> SearchDepartmentsByName(string keyword)
        {
            ArgumentNullException.ThrowIfNull(keyword);
            return _dbContext.Departments
                .Where(d => d.Name.Contains(keyword))
                .ToList();
        }

    }
}
