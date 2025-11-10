using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestSample01.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name cannot be null or empty.")]
        [MaxLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Department description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
