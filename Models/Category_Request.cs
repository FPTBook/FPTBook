using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Models
{
    public class Category_Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string user_id { get; set; }
        [ForeignKey("user_id")]
        public virtual User? user { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
        [DefaultValue(0), Range(0,2)]
        public int status { get; set; }
    }
}