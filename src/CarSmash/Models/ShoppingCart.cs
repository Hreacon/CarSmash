using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CarSmash.Models
{
    [NotMapped]
    [Serializable]
    public class ShoppingCart
    {
        public List<Product> Products { get; set; }
        public List<int> Quantity { get; set; }

        public ShoppingCart()
        {
            Products = new List<Product>();
            Quantity = new List<int>();
        }

        public bool Any() => Products.Any();

        public int Count
        {
            get
            {
                int total = 0;
                foreach (int count in Quantity)
                {
                    total = total + count;
                }
                return total;
            }
        }

        public void Add(Product item)
        {
            if (Products.Contains(item))
            {
                Quantity[Products.IndexOf(item)]++;
            }
            else
            {
                Products.Add(item);
                Quantity.Add(1);
            }
        }
    }
}
