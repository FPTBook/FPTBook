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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> userManager;
        public CustomerController( ApplicationDbContext context, UserManager<User> userManager)
        {
            this._context = context;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Profile(string username)
        {
            if (username == null || userManager.Users == null)
            {
                return NotFound();
            }
            var result = await _context.Users.Include(u => u.orders).FirstOrDefaultAsync(u => u.UserName == username);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        public async Task<IActionResult> Update(string id)
        {
            if (id == null || userManager.Users == null)
            {
                return NotFound();
            }

            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Update(User model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            var userExists = await userManager.FindByNameAsync(model.UserName);
            var emailExist = await userManager.FindByEmailAsync(model.Email);
            if(userExists != null && userExists.UserName != user.UserName){
                TempData["war"] = "The username has already existed!";
                return RedirectToAction(nameof(Update));
            }

            if (emailExist != null && emailExist.Email != user.Email)
            {
                TempData["war"] = "The email has already existed!";
                return RedirectToAction(nameof(Update));
            }
            if (user == null)
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

            if (result.Succeeded)
            {
                TempData["msg"] = "Updated successfully!";
                return RedirectToAction(nameof(Update));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult CancelOrder(int id)
        {
            var order = _context.Orders.Find(id);
            var orderDetail = _context.OrderDetails.Where(od => od.order_id == order.id).ToList();
            foreach (var item in orderDetail)
            {
                var itemBook = _context.Books.ToList().Where(b => b.id == item.book_id).FirstOrDefault();
                itemBook.quantity = itemBook.quantity + item.book_quantity;
                _context.Update(itemBook);
            }
            order.status = 2;
            _context.Update(order);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}