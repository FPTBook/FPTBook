using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FPTBook.DB;
using FPTBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FPTBook.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            this._db = db;
        }

        public IActionResult Index(string id)
        {
            var lstCart = GetLstCart();
            string userId = GetUserId();
            var cart = lstCart.Where(c => c.user_id == userId).ToList();
            return View(cart);
        }
        private string GetUserId()
        {
            return Convert.ToString(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        private List<Cart> GetLstCart()
        {
            return _db.Carts.Include(c => c.user).Include(c => c.book).ToList();
        }
    }
}