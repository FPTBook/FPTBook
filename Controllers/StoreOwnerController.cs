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
            var lstBook = _db.Books.Include(b => b.category).ToList();

            return View(lstBook);
        }

        [HttpGet]
        public IActionResult ViewListBooks(string keyword)
        {
            var lstBook = _db.Books.Include(b => b.category).ToList();
            if (lstBook == null)
            {
                return NotFound();
            }
            var books = from b in _db.Books select b;
            if (!String.IsNullOrEmpty(keyword))
            {
                books = books.Where(s => s.status == 1 && s.name!.Contains(keyword));
            }

            return View(books.ToList());
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
                        ViewData["category_id"] = new SelectList(categories, "id", "name");
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

        public IActionResult UpdateBook(int id)
        {
            var book = _db.Books.Find(id);
            var categories = _db.Categories.Where(c => c.status == 1).ToList();
            ViewData["category_id"] = new
            // SelectList(_db.Categories, "Id", "Id", book.category_id)
            SelectList(categories, "id", "name");
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBook(IFormFile? img, Book book)
        {
            var categories = _db.Categories.Where(c => c.status == 1).ToList();
            if (ModelState.IsValid)
            {
                var filePaths = new List<string>();
                if (img != null && img.Length > 0)
                {
                    string fileType = Path.GetExtension(img.FileName).ToLower().Trim();
                    if (fileType != ".jpg" && fileType != ".png")
                    {
                        TempData["msg"] = "File Format Not Supported. Only .jpg and .png !";
                        ViewData["category_id"] = new SelectList(categories, "id", "name");
                        return View(book);
                    }

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", img.FileName);
                    book.image = img.FileName;
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }
                    if (book.is_deleted)
                    {
                        book.status = 2;
                    }
                    TempData["msg"] = "Updated successfully!";
                    _db.Update(book);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(UpdateBook));
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
                    // var od = _db.OrderDetails.Where(od => od.order_id == order.id).ToList();
                    // foreach (var item in od)
                    // {
                    //     var book = _db.Books.Where(b => b.id == item.book_id).FirstOrDefault();
                    //     book.quantity = book.quantity - item.book_quantity;
                    //     if(book.quantity == 0)
                    //     {
                    //         book.status = 0;
                    //     }
                    //     _db.Update(book);

                    // }
                    // _db.SaveChanges();
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

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCategory(Category category, string username)
        {
            if (ModelState.IsValid)
            {
                category.status = 0;
                _db.Add(category);


                var user = _db.Users.Where(u => u.UserName == username).FirstOrDefault();
                _db.Add(new Category_Request
                {
                    user_id = user.Id,
                    user = user,
                    name = category.name,
                    date = DateTime.Now,
                    status = 0
                });
                _db.SaveChanges();
                return RedirectToAction(nameof(ViewListCategories));
            }
            return View(category);
        }

        public IActionResult UpdateCategory(int id)
        {
            var category = _db.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category model)
        {
            var category = _db.Categories.Find(model.id);
            if (ModelState.IsValid)
            {
                category.name = model.name;
                category.description = model.description;
                _db.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(ViewListCategories));
            }
            return View(category);
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = _db.Categories.Find(id);
            category.status = 3;
            var books = _db.Books.Where(b => b.category_id == category.id).ToList();
            foreach (var item in books)
            {
                item.status = 2;
                _db.Update(item);
            }
            _db.Update(category);
            _db.SaveChanges();
            return RedirectToAction(nameof(ViewListCategories));
        }

        public IActionResult BestSellerBook()
        {
            // var order = _db.Orders.Include(o => o.orders_detail).Include(o => o.orders_detail);
            // var orderDetail = _db.OrderDetails.Include(od => od.book).Include(od => od.order).Where(od => od.order.status == 1).ToList();
            var books = _db.Books.Include(b => b.category).Include(b => b.orders_detail).
            Where(b => b.orders_detail.FirstOrDefault().order.status == 1).
            OrderByDescending(b => b.orders_detail.Where(o => o.book_id == b.id && o.order.status == 1).
            Sum(o => o.book_quantity)).ToList();
            return View(books);
        }

    }
}