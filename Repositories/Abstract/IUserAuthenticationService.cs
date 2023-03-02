using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPTBook.Models.DTO;

namespace FPTBook.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        Task<Status> LoginAsync(LoginModel model);
        Task LogoutAsync();
        Task<Status> RegistrationAsync(RegistrationModel model);
        Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username);
        Task<Status> ResetPasswordAsync(ResetPasswordModel model, string username);
    }
}