/* Assignment 5 Webtech
 * 
 * Ezatullah Rafie
 * 
 * December 5, 2020
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERClubs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERClubs.Controllers
{
    [Authorize(Roles = "administrator")]
    public class ERRoleController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ERRoleController(
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var roles = roleManager.Roles.OrderBy(x => x.Name);
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(ERRole model)
        {
            var name = model.RoleName + "";
            try
            {
                name = name.Trim();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = name
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    ViewBag.Success = "Successfully added: " + name;
                    return RedirectToAction("Index");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> RoleMembers(string Name)
        {
            List<string> notEnrolled = new List<string>();
            try
            {
                ViewBag.RoleName = Name;
                var users = await userManager.GetUsersInRoleAsync(Name);
                var allUsers = userManager.Users.ToList();
                foreach (var user in allUsers)
                {
                    if (!await userManager.IsInRoleAsync(user, Name))
                    {
                        notEnrolled.Add(user.UserName);
                    }
                }
                ViewBag.Enrolled = new SelectList(notEnrolled);
                return View(users.OrderBy(x => x.UserName).ToList());
            }
            catch (Exception)
            {
                TempData["errorMessage"] = "No member was found";
                return RedirectToAction("RoleMembers");
            }
        }
        public async Task<IActionResult> Remove(string userName, string roleName)
        {
            string currentUser = User.Identity.Name;
            if (!String.IsNullOrEmpty(currentUser))
            {
                IdentityUser currentUserInfo = await userManager.FindByNameAsync(currentUser);
                var currentUserRole = await userManager.GetRolesAsync(currentUserInfo);
                if (currentUserRole.Contains("administrator"))
                {
                    TempData["errorMessage"] = "you cannot delete your role from admin";
                    return RedirectToAction("Index");
                }
            }
            try
            {
                IdentityUser thisUser = await userManager.FindByNameAsync(userName);
                await userManager.RemoveFromRoleAsync(thisUser, roleName);
                TempData["errorMessage"] = "successfully removed";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddToRole(string UserName, string name)
        {
            try
            {
                IdentityUser currentUser = await userManager.FindByNameAsync(UserName);
                await userManager.AddToRoleAsync(currentUser, name);
                TempData["errorMessage"] = "Successfully added";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string roleName)
        {
            try
            {
                var users = await userManager.GetUsersInRoleAsync(roleName);
                if (users.Count != 0)
                {
                    ViewBag.Users = users;
                    return View();
                }
                else
                {
                    if (await roleManager.RoleExistsAsync(roleName))
                    {
                        IdentityRole roleToDelete = await roleManager.FindByNameAsync(roleName);
                        await roleManager.DeleteAsync(roleToDelete);
                        TempData["errorMessage"] = "Successfully Deleted";
                        return RedirectToAction("Index");
                    } 
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            try
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    IdentityRole roleToDelete = await roleManager.FindByNameAsync(roleName);
                    await roleManager.DeleteAsync(roleToDelete);
                    TempData["errorMessage"] = "Successfully Deleted";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
