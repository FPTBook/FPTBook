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

        [HttpPost]
        public async Task<IActionResult> AddToCart(Book model, int book_id)
        {
            if(model.quantity < 1)
            {
                TempData["msg"] = "The number of quantity book to add to cart is a number greater than 1!";
                return RedirectToAction("Detail", "Home");
            }
            var book = await _db.Books.Where(b => b.id == book_id).FirstOrDefaultAsync();
            string userId = GetUserId();
            var user = await _db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            var lstCart = GetLstCart();
            var cartItem = lstCart.Find(b => b.book_id == book_id && b.user_id == userId);
            if (cartItem != null)
            {
                cartItem.quantity = cartItem.quantity + model.quantity;
                _db.Update(cartItem);
                await _db.SaveChangesAsync();
            }
            else
            {
                _db.Add(new Cart()
                {
                    book_id = book_id,
                    book = book,
                    user = user,
                    user_id = userId,
                    quantity = model.quantity,
                    date = DateTime.Now
                });
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
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

            var lstCart = GetLstCart();
            var cartItem = lstCart.Find(b => b.book_id == book_id && b.user_id == userId);
            if (cartItem != null)
            {
                cartItem.quantity++;
                _db.Update(cartItem);
                await _db.SaveChangesAsync();
            }
            else
            {
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
            }
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

        public IActionResult Checkout()
        {
            var lstCart = _db.Carts.Include(c => c.user).Include(c => c.book).Include(c => c.book.category).ToList();
            string userId = GetUserId();
            var cart = lstCart.Where(c => c.user_id == userId).ToList();
            if (cart.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        public IActionResult Order(double total)
        {
            var lstCart = GetLstCart();
            string userId = GetUserId();
            var cart = lstCart.Where(c => c.user_id == userId).ToList();
            var user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
            var book = _db.Books.ToList();
            
            foreach (var item in cart)
            {
                var itemBook = book.Where(b => b.id == item.book_id).FirstOrDefault();
                var temp = itemBook.quantity;
                temp = temp - item.quantity;
                if (temp < 0)
                {
                    TempData["msg"] = "Book with name " + itemBook.name + " has not enough quantity to buy!";
                    return RedirectToAction(nameof(Checkout));
                }
            }

            _db.Add(new Order
            {
                user = user,
                user_id = userId,
                order_date = DateTime.Now,
                address = user.address,
                payment = total,
                status = 0
            });
            _db.SaveChanges();

            var order = _db.Orders.OrderByDescending(o => o.id).Where(o => o.user_id == userId).FirstOrDefault();

            foreach (var item in cart)
            {
                _db.Add(new OrderDetail
                {
                    book = item.book,
                    book_id = item.book_id,
                    order = order,
                    order_id = order.id,
                    book_quantity = item.quantity,
                    book_price = item.book.price,
                    total = item.quantity * item.book.price

                });
                var itemBook = book.Where(b => b.id == item.book_id).FirstOrDefault();
                itemBook.quantity = itemBook.quantity - item.quantity;
                if(itemBook.quantity == 0)
                {
                    itemBook.status = 0;
                }
                _db.Update(itemBook);
            }
            _db.SaveChanges();

            foreach (var item in cart)
            {
                _db.Carts.Remove(item);
            }
            _db.SaveChanges();

            return RedirectToAction("Index", "Home");
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