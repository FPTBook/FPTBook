using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int book_id { get; set; }
        [ForeignKey("book_id")]
        public virtual Book? book { get; set; }
        public string user_id { get; set; }
        [ForeignKey("user_id")]
        public virtual User? user { get; set; }
        [Range(0, int.MaxValue)]
        public int quantity { get; set; }
        public DateTime date { get; set; }

    }
}