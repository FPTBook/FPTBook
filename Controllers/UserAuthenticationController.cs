using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FPTBook.Models.DTO;
using FPTBook.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FPTBook.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _service;
        public UserAuthenticationController(IUserAuthenticationService service)
        {
            this._service = service;
        }
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            model.Role = "customer";
            var result = await _service.RegistrationAsync(model);
            TempData["msg"] = result.Message;
            return RedirectToAction(nameof(Registration));
        }
        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _service.LoginAsync(model);
            if(result.StatusCode == 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["msg"] = result.Message;
                return RedirectToAction(nameof(Login));
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        
        // public async Task<IActionResult> Reg()
        // {
        //     var model = new RegistrationModel
        //     {
        //         Full_Name = "Luong Gia Luan",
        //         Username = "admin",
        //         Email = "admin@gmail.com",
        //         Gender = "Male",
        //         Phone = "0989999999",
        //         Address = "Can Tho",
        //         Password = "Admin@123"
                
        //     };
        //     model.Role = "admin";
        //     var result = await _service.RegistrationAsync(model);
        //     TempData["msg"] = result.Message;
        //     return Ok(result);
        // }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult>ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
              return View(model);
            var result = await _service.ChangePasswordAsync(model, User.Identity.Name);
            TempData["msg"] = result.Message;
            return RedirectToAction("ChangePassword", "Customer");
        }

        
    }
}