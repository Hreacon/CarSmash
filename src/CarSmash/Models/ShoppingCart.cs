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

        public ShoppingCart()
        {
            Products = new List<Product>();
        }

        public bool Any() => Products.Any();

        public int Count
        {
            get
            {
                int total = 0;
                foreach (var product in Products)
                {
                    total = total + product.Quantity;
                }
                return total;
            }
        }

        public void Add(Product item)
        {
            int index = Products.IndexOf(item);
            if (index >= 0)
            {
                Products[index].Quantity++;
            }
            else
            {
                item.Quantity = 1;
                Products.Add(item);
            }
        }
    }
}
