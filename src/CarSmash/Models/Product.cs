using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CarSmash.Models
{
    [Serializable]
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public virtual List<Image> Images { get; set; }

        [NotMapped]
        public int Quantity { get; set; }

        public Product()
        {
            Quantity = 0;
        }

        public override bool Equals(object obj)
        {
            Product other = obj as Product;
            if (other?.ProductId == ProductId) return true;
            return false;
        }
    }
}