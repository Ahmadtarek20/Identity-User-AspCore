using EmpoolysMangment.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmpoolysMangment.ViewModel
{
    public class EmployeeCreatViewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Name is not MaxLength 50 char")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "OFFice Email")]
        public string Email { get; set; }
        [Required]
        public Department? Department { get; set; }
        public IFormFile Photo { get; set; }
    }
}
