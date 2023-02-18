using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FPTBook.Models
{
    public class User : IdentityUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        [Required]
        public string full_name { get; set; }
        [Required]
        public DateTime birthday { get; set; }
        [Required]
        public string gender { get; set; }
        [Required]
        public string address { get; set; }
        [DefaultValue(1), Range(0,2)]
        public int status { get; set; }
        public virtual ICollection<Cart>? carts { get; set; }
        public virtual ICollection<Category_Request>? cat_requests { get; set; }
        public virtual ICollection<Order>? orders { get; set; }
    }
}