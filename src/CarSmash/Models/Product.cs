using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.AspNet.Mvc;

namespace CarSmash.Models
{
    [Table ("Products")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string Desc { get; set; }
        public double Price { get; set; }
        public virtual ICollection<Image> Images { get; set; }

    }
}
