using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Models.DTO
{
    public class UpdateUserModel
    {
        public UpdateUserModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
        public string Id { get; set; }
        [Required]
        public string Full_Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        public List<string> Claims { get; set; }
        public IList<string> Roles { get; set; }
    }
}