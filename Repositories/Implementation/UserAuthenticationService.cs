using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FPTBook.Models;
using FPTBook.Models.DTO;
using FPTBook.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;

namespace FPTBook.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public UserAuthenticationService(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username)
        {
            var status = new Status();

            var user =  await userManager.FindByNameAsync(username);
            if(user == null)
            {
                status.Message = "User does not exist";
                status.StatusCode = 0;
                return status;
            }
            
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if(result.Succeeded)
            {
                status.Message = "Password has updated successfully!";
                status.StatusCode = 1;
            }
            else
            {
                status.Message = "Some error occured";
                status.StatusCode = 0;
            }

            return status;
        }

        public async Task<Status> ResetPasswordAsync(ResetPasswordModel model, string username)
        {
            var status = new Status();

            var user =  await userManager.FindByNameAsync(username);
            if(user == null)
            {
                status.Message = "User does not exist";
                status.StatusCode = 0;
                return status;
            }

            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, code.ToString(), model.NewPassword);
            if(result.Succeeded)
            {
                status.Message = "Account has reset password successfully!";
                status.StatusCode = 1;
            }
            else
            {
                status.Message = "Some error occured";
                status.StatusCode = 0;
            }
            return status;
        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByEmailAsync(model.Email);
            if(user == null){
                status.StatusCode = 0;
                status.Message = "Invalid email or password";
                return status;
            }

            if(!await userManager.CheckPasswordAsync(user, model.Password)){
                status.StatusCode = 0;
                status.Message = "Invalid email or password";
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if(signInResult.Succeeded){
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>{
                    new Claim(ClaimTypes.Name, user.Email)
                };
                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.Message = "Logged in successfully!";
                return status;
            }
            else if(signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User locked out";
                return status;
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error on login";
                return status;
            }
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrationAsync(RegistrationModel model)
        {
            var status = new Status();
            var userExists = await userManager.FindByNameAsync(model.Username);
            var emailExist = await userManager.FindByEmailAsync(model.Email);
            if(userExists != null || emailExist != null){
                status.StatusCode = 0;
                status.Message = "User already existed";
                return status;
            }

            User user = new User{
                SecurityStamp = Guid.NewGuid().ToString(),
                full_name = model.Full_Name,
                UserName = model.Username,
                Email = model.Email,
                gender = model.Gender,
                PhoneNumber = model.Phone,
                address = model.Address,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Role = model.Role,
            };

            var result = await userManager.CreateAsync(user,model.Password);
            if(!result.Succeeded){
                status.StatusCode = 0;
                status.Message = "User register failed";
                return status;
            }

            //role
            if(!await roleManager.RoleExistsAsync(model.Role)){
                await roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            if(await roleManager.RoleExistsAsync(model.Role)){
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            status.Message = "User has registered successfully!";
            return status;
        }
    }
}