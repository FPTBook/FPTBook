using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FPTBook.DB;
using FPTBook.Models.DTO;
using FPTBook.Models;
using FPTBook.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace FPTBook.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        // private readonly ApplicationDbContext _context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public CustomerController(
            // ApplicationDbContext context,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager
        )
        {
            // this._context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Profile(string username)
        {
            if(username == null || userManager.Users == null)
            {
                return NotFound();
            }
            var result = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if(result == null)
            {
                return NotFound();
            }
            return View(result);
        }
        
        public async Task<IActionResult> Update(string id)
        {
            if(id == null || userManager.Users == null)
            {
                return NotFound();
            }

            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Update(User model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if(user == null)
            {
                return NotFound();
            }
            else
            {
                user.full_name = model.full_name;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.gender = model.gender;
                user.PhoneNumber = model.PhoneNumber;
                user.PhoneNumberConfirmed = true;
                user.address = model.address;

            }
            var result = await userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                TempData["msg"] = "Updated successfully!";
                return RedirectToAction(nameof(Update));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("",error.Description);
            }

            return View(model);
        }
    }
}