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

        public async Task<IActionResult> AddCart(int book_id)
        {
            var book = await _db.Books.Where(b => b.id == book_id).FirstOrDefaultAsync();
            string userId = GetUserId();
            var user = await _db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            _db.Add(new Cart()
            {
                book_id = book_id,
                book = book,
                user = user,
                user_id = userId,
                quantity = 1,
                date = DateTime.Now
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteCart(int? id)
        {
            var cartItem = await _db.Carts.FindAsync(id);
            if (cartItem != null)
            {
                _db.Carts.Remove(cartItem);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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