using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int category_id { get; set; }
        [ForeignKey("category_id")]
        public virtual Category? category { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string author { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        public string description { get; set; }
        [Required, Range(1, 100000)]
        public int quantity { get; set; }
        [Required, Range(1.0, 100000.0)]
        public double old_price { get; set; }
        [Required, Range(1.0, 100000.0)]
        public double price { get; set; }
        [Required]
        public string image { get; set; } = "img";
        [DefaultValue(false)]
        public bool is_deleted { get; set;}
        [DefaultValue(1), Range(0,2)]
        public int status { get; set; }
        public virtual ICollection<Cart>? carts { get; set; }
        public virtual ICollection<OrderDetail>? orders_detail { get; set; }
    }
}