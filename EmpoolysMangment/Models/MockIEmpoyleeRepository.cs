using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpoolysMangment.Models
{
    public class MockIEmpoyleeRepository : IEmpoyleeRepository
    {
        private List<Employee> _employeeslist;
        public MockIEmpoyleeRepository()
        {
            
        }

        public Employee Add(Employee employee)
        {
          employee.Id =  _employeeslist.Max(e => e.Id) + 1;
            _employeeslist.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = _employeeslist.FirstOrDefault(e => e.Id == id);
            if(employee != null)
            {
                _employeeslist.Remove(employee);
            }
            return employee;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeslist.FirstOrDefault(e => e.Id == Id);
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return _employeeslist;
        }

        public Employee Update(Employee employeeChenge)
        {
            Employee employee = _employeeslist.FirstOrDefault(e => e.Id == employeeChenge.Id);
            if (employee != null)
            {
                employee.Name = employeeChenge.Name;
                employee.Email = employeeChenge.Email;
                employee.Department = employeeChenge.Department;

            }
            return employee;
        }
    }
}
