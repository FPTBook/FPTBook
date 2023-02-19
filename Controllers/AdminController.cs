using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FPTBook.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            return View();
        }
        
    }
}