using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FPTBook.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FPTBook.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db)
        {
            this._db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ViewListAccounts()
        {
            var lstAcc = _db.Users.ToList();
            return View(lstAcc);
        }
    }
    
}