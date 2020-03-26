using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmpoolysMangment.Models;
using EmpoolysMangment.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EmpoolysMangment.Controllers
{
    public class HomeController : Controller
    {
        private IEmpoyleeRepository _empoyleeRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IEmpoyleeRepository empoyleeRepository,
                                  IHostingEnvironment hostingEnvironment)
        {
            _empoyleeRepository = empoyleeRepository;
            this.hostingEnvironment = hostingEnvironment;
        } 
        public ViewResult Index()
        {
           var model = _empoyleeRepository.GetEmployees();
            return View(model);
        }
        public ViewResult Details(int? id)
        {
           // throw new Exception("Error is Detaild View");

            Employee employee = _empoyleeRepository.GetEmployee(id.Value);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound" , id.Value);
            }
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
            Employs = employee,
            PageTitle = "Employee De"
            };  
            return  View(homeDetailsViewModel);

        }
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeCreatViewModel model)
        {
            
            if(ModelState.IsValid)
            {
                string uniqfilename = PrograsUplodFile(model);
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqfilename
                };
                _empoyleeRepository.Add(newEmployee);
                return RedirectToAction("Details", new { id = newEmployee.Id });
            }
            return View();
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _empoyleeRepository.GetEmployee(id);
            EmplooysEditViewModel emplooysEditViewModel = new EmplooysEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(emplooysEditViewModel);
        }
        [HttpPost]
        public IActionResult Edit(EmplooysEditViewModel model)
        {

            if (ModelState.IsValid)
            {
                Employee employee = _empoyleeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if(model.Photo != null)
                {
                    if(model.ExistingPhotoPath != null)
                    {
                        string filepath = 
                            Path.Combine(hostingEnvironment.WebRootPath, "images",model.ExistingPhotoPath);
                        System.IO.File.Delete(filepath);
                    }
                    employee.PhotoPath = PrograsUplodFile(model);
                }
                _empoyleeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View();
        }

        private string PrograsUplodFile(EmployeeCreatViewModel  model)
        {
            string uniqfilename = null;
            if (model.Photo != null)
            {


                string uplodsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqfilename = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filepath = Path.Combine(uplodsFolder, uniqfilename);
                model.Photo.CopyTo(new FileStream(filepath, FileMode.Create));

            }

            return uniqfilename;
        }
    }
}