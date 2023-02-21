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
    }
}