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
    [Authorize(Roles = "storeowner")]
    public class StoreOwnerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StoreOwnerController(ApplicationDbContext db)
        {
            this._db = db;
        }
        public IActionResult ViewListBooks()
        {
            var lstBook = _db.Books.ToList();

            return View(lstBook);
        }
    }
}