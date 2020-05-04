using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public async Task<IActionResult> Login(string reternurl)
        {

            loginViewModel model = new loginViewModel
            {
                RerernUrl = reternurl,
                ExternalLogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
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
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider , string reternurl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Acount",
                new { RerernUrl = reternurl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }
        public async Task<IActionResult> ExternalLoginCallback(string reternUrl = null , string remoteError = null)
        {
            reternUrl = reternUrl ?? Url.Content("~/");

            loginViewModel loginViewModel = new loginViewModel
            {
                RerernUrl = reternUrl,
                ExternalLogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if(remoteError != null)
            {
                ModelState.AddModelError(string.Empty,$"Error from external provider:{remoteError}");
                return View("Login", loginViewModel);
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if(info == null)
            {
                ModelState.AddModelError(string.Empty, "Eroor loding extenal login ingormaion");
                return View("Login", loginViewModel);
            }
            var signalResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signalResult.Succeeded)
            {
                return LocalRedirect(reternUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null) 
                {
                    var user = await userManger.FindByEmailAsync(email);
                    if(user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await userManger.CreateAsync(user);
                    }

                    await userManger.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(reternUrl);
                }
                ViewBag.ErrorTitle = $"Email clims not recived form :{info.LoginProvider}";
                ViewBag.ErrorMessage = $"Please contact suport on Test@Yahoo.com";
                return View("Error");

            }
            return View("Login", loginViewModel);
        }
    }
}