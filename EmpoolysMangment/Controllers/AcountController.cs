using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmpoolysMangment.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmpoolysMangment.Controllers
{
    public class AcountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManger;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AcountController(UserManager<ApplicationUser> userManger,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManger = userManger;
            this.signInManager = signInManager;
        }
        [HttpPost]
        public async Task<IActionResult> logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(loginViewModel model , string reternurl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password
                    , model.RememberMe , false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(reternurl)&& Url.IsLocalUrl(reternurl))
                    {
                        return Redirect(reternurl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");

                    }
                }
                 ModelState.AddModelError(string.Empty, "Invalid login Attempt");

                
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
               var user = new ApplicationUser { UserName = model.Email, Email = model.Email , city = model.city };
                var result = await userManger.CreateAsync(user, model.Password);

                  if(result.Succeeded)
                {
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Adminstration");
                    }
                   await signInManager.SignInAsync(user, isPersistent:false);
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