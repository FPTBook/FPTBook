using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FPTBook.DB;
using FPTBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult CreateBook()
        {
            var categories = _db.Categories.Where(c => c.status == 1).ToList();
            ViewData["category_id"] = new SelectList(categories, "id", "name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(IFormFile img, Book book)
        {
            var categories = _db.Categories.Where(c => c.status == 1).ToList();
            if (ModelState.IsValid)
            {
                var filePaths = new List<string>();
                if (img.Length > 0)
                {
                    string fileType = Path.GetExtension(img.FileName).ToLower().Trim();
                    if (fileType != ".jpg" && fileType != ".png")
                    {
                        TempData["msg"] = "File Format Not Supported. Only .jpg and .png !";
                        return View(book);
                    }

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", img.FileName);
                    book.image = img.FileName;
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }

                    _db.Add(book);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(ViewListBooks));
                }

            }
            ViewData["category_id"] = new SelectList(categories, "id", "name");
            return View(book);
        }

        public IActionResult DetailBook(int id)
        {
            var book = _db.Books.Find(id);
            if (book == null)
            {
                return RedirectToAction("Index");
            }

            return View(book);
        }

        public IActionResult DeleteBook(int id)
        {
            Book book = _db.Books.Find(id);
            if (book == null)
            {
                return RedirectToAction("Index");
            }
            book.is_deleted = true;
            book.status = 2;
            _db.Update(book);
            _db.SaveChanges();
            return RedirectToAction(nameof(ViewListBooks));
        }

        public IActionResult ViewListOrders()
        {
            var lstOrder = _db.Orders.Include(o => o.user).ToList();
            if (lstOrder == null)
            {
                return NotFound();
            }
            return View(lstOrder);
        }
        public IActionResult UpdateOrder(int id)
        {
            var order = _db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateOrder(Order model)
        {
            var order = _db.Orders.Find(model.id);
            if (model.status != order.status || model.address != order.address)
            {
                if (model.status == 1)
                {
                    order.delivery_date = DateTime.Now;
                }

                if (model.status == 0)
                {
                    order.delivery_date = null;
                }
                order.address = model.address;
                order.status = model.status;
                _db.Update(order);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(ViewListOrders));
        }
          public IActionResult ViewOrderDetail(int id)
        {
            var lstOrderDetail = _db.OrderDetails.Include(od => od.book).Where(od => od.order_id == id).ToList();
            if (lstOrderDetail == null)
            {
                return NotFound();
            }
            return View(lstOrderDetail);
        }
        public IActionResult ViewListCategories()
        {
            var categories = _db.Categories.ToList();
            return View(categories);
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = _db.Categories.Find(id);
            category.status = 0;
            _db.Update(category);
            _db.SaveChanges();
            return View();
        }

    }
}