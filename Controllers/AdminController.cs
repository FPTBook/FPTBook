using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FPTBook.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult CategoryApproval()
        {
            var lstApp = _db.Category_Requests.Include(c => c.user).ToList();
            return View(lstApp);
        }

        public IActionResult ApproveRequest(int id)
        {
            var obj = _db.Category_Requests.Find(id);
            obj.status = 1;
            var cate = _db.Categories.Where(c => c.name == obj.name).FirstOrDefault();
            cate.status = 1;
            _db.Update(cate);
            _db.Update(obj);
            _db.SaveChanges();
            return RedirectToAction(nameof(CategoryApproval));
        }
        
        public IActionResult RejectRequest(int id)
        {
            var obj = _db.Category_Requests.Find(id);
            obj.status = 1;
            var cate = _db.Categories.Where(c => c.name == obj.name).FirstOrDefault();
            cate.status = 2;
            _db.Update(cate);
            _db.Update(obj);
            _db.SaveChanges();
            return RedirectToAction(nameof(CategoryApproval));
        }
    }
    
}