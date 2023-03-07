using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions; 

namespace FPTBook.Models
{
    public class User : IdentityUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        [Required]
        [StringLength(30)]
        [RegularExpression("^([^0-9]*)$", ErrorMessage = "Invalid Full Name.")]
        public string full_name { get; set; }
        [Required]
        [StringLength(20)]
        public override string UserName { get => base.UserName; set => base.UserName = value; }
        [Required]
        public DateTime birthday { get; set; }
        [Required]
        public string gender { get; set; }
        [Required]
        [StringLength(200)]
        public string address { get; set; }
        [DefaultValue(1), Range(0,2)]
        public int status { get; set; }
        [Required]
        [DefaultValue("customer")]
        public string Role { get; set; }
        [Required]
        [RegularExpression("^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public override string Email { get => base.Email; set => base.Email = value; }
        public virtual ICollection<Cart>? carts { get; set; }
        public virtual ICollection<Category_Request>? cat_requests { get; set; }
        public virtual ICollection<Order>? orders { get; set; }
    }
}