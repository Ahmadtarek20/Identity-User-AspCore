using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmpoolysMangment.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmpoolysMangment.Controllers
{
    public class AcountController : Controller
    {
        public UserManager<IdentityUser> UserManger { get; }
        public SignInManager<IdentityUser> SignInManager { get; }

        public AcountController(UserManager<IdentityUser>userManger,
            SignInManager<IdentityUser>signInManager)
        {
            UserManger = userManger;
            SignInManager = signInManager;
        }

       
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
               var user = new IdentityUser { UserName = model.Email, Email = model.Email };
               var result = await UserManger.CreateAsync(user, model.Password);
                  if(result.Succeeded)
                {
                   await SignInManager.SignInAsync(user, isPersistent:false);
                    return RedirectToAction("index", "home");

                }
                  foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);

                }
            }
            return View(model);
        }
    }
}