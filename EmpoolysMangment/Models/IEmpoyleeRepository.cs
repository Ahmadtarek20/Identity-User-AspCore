using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpoolysMangment.Models
{
    public interface IEmpoyleeRepository
    {
        Employee GetEmployee(int Id);
        IEnumerable<Employee> GetEmployees();
        Employee Add(Employee employee);
        Employee Update(Employee employee);
        Employee Delete(int id);
    }
}
