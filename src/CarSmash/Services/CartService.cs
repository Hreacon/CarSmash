using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using CarSmash.Models;
using Microsoft.AspNet.Http;

namespace CarSmash.Services
{
    public class CartService
    {
        public static ShoppingCart GetCart(HttpContext context)
        {
            ShoppingCart _cart;
            string cart = context.Session.GetString("ShoppingCart");
            if (cart != null)
            {
                byte[] bytes = Convert.FromBase64String(cart);
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(bytes, 0, bytes.Length);
                    ms.Position = 0;
                    _cart = new BinaryFormatter().Deserialize(ms) as ShoppingCart;
                }
            }
            else
            {
                _cart = new ShoppingCart();
            }
            return _cart;
        }
    }
}
