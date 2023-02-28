using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string user_id { get; set; }
        [ForeignKey("user_id")]
        public virtual User? user { get; set; }
        public DateTime order_date { get; set; }
        public DateTime? delivery_date { get; set; }
        public string address { get; set; }
        [DefaultValue(0), Range(0, double.MaxValue)]
        public double payment { get; set; }
        [DefaultValue(1), Range(0,2)]
        public int status { get; set; }
        public virtual ICollection<OrderDetail>? orders_detail { get; set; }
    }
}