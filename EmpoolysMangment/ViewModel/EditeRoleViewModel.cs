using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmpoolysMangment.ViewModel
{
    public class EditeRoleViewModel
    {
        public EditeRoleViewModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }
        [Required(ErrorMessage ="Role Name is requierd")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}
