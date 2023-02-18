using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int book_id { get; set; }
        [ForeignKey("book_id")]
        public virtual Book? book { get; set; }
        public int order_id { get; set; }
        [ForeignKey("order_id")]
        public virtual Order? order { get; set; }
        [Range(0, int.MaxValue)]
        public int book_quantity { get; set; }
        [Range(0, double.MaxValue)]
        public double book_price { get; set; }
        [Range(0, double.MaxValue)]
        public double total { get; set; }
    }
}