using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FPTBook.DB;
using FPTBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    }
}