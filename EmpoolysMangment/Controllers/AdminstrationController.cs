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
    //[Authorize(Roles ="Admin")]
    public class AdminstrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminstrationController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
       
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }



        [HttpGet]
        public async Task<IActionResult> MangeRols(string userid)
        {
            ViewBag.userid = userid;
            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorManager = $"User with Id = {userid} cannot be found";
                return View("Not found");
            }

            var model = new List<UserRoleslViewModel>();
            foreach(var rol in roleManager.Roles)
            {
                var userRoleslViewModel = new UserRoleslViewModel
                {
                    RoleId = rol.Id,
                    RoleName = rol.Name,
                };
                if(await userManager.IsInRoleAsync(user, rol.Name))
                {
                    userRoleslViewModel.IsSelected = true;
                }
                else
                {
                    userRoleslViewModel.IsSelected = false;
                }
                model.Add(userRoleslViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorManager = $"User with Id = {id} cannot be found";
                return View("Not found");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("ListUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletRoles(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorManager = $"User with Id = {id} cannot be found";
                return View("Not found");
            }
            else
            {
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("ListRole");
            }
        }


        [HttpGet]
        public async Task<IActionResult>  EditUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                ViewBag.ErrorManager = $"User with Id = {Id} cannot be found";
                return View("Not found");
            }
            var userClamis = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                city = user.city,
                Claims = userClamis.Select(c => c.Value).ToList(),
                Roles = userRoles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorManager = $"User with Id = {model.Id} cannot be found";
                return View("Not found");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.city = model.city;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
           
        }

        [HttpGet]
        public IActionResult CreatRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatRole(CreatRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole", "Adminstration");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ListRole()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorManager = $"Role with Id = {id} cannot be found";
                return View("Not found");
            }
            var model = new EditeRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditeRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorManager = $"Role with Id = {model.Id} cannot be found";
                return View("Not found");
            }
            else
            {
                role.Name = model.RoleName;
                var reselt = await roleManager.UpdateAsync(role);
                if (reselt.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }
                foreach (var error in reselt.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorManager = $"Role with Id = {roleId} cannot be found";
                return View("Not found");
            }
            var model = new List<UserRoleViewModel>();
            foreach (var user in userManager.Users)
            {
                var userRoleviewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleviewModel.IsSelected = true;
                }
                else
                {
                    userRoleviewModel.IsSelected = false;
                }
                model.Add(userRoleviewModel);
            }
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorManager = $"Role with Id = {roleId} cannot be found";
                return View("Not found");
            }
            for(int i= 0; i < model.Count; i++)
            {
               var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                   result= await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole ", new { Id = roleId });
                }
            }
            return RedirectToAction("EditRole ", new { Id = roleId });
        }
        
        }
}