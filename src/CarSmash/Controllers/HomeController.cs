using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Mvc;
using CarSmash.Models;
using Microsoft.Data.Entity;

namespace CarSmash.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _db { get; }
        private ShoppingCart _cart { get; set; }

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
            
        }
       
        public IActionResult Index()
        {
            if (!_db.Products.Any())
            {
                // if no products create a test product
                //TODO remove on deploy
                _db.Images.Add(new Image()
                {
                    Url = "/images/products/nurburg.jpg"
                });
                _db.SaveChanges();
                _db.Products.Add(new Product()
                {
                    Description = "Nurburgring Sticker",
                    Images = _db.Images.ToList(),
                    Name = "Nurburg Sticker",
                    Price = 10.99
                });
                _db.SaveChanges();
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Products()
        {
            GetCart();
            var products = await _db.Products.Include(m=>m.Images).ToListAsync();
            return View(products);
        }

        public IActionResult ProductDetails(int id)
        {
            return View("Details", _db.Products.FirstOrDefault(m=>m.ProductId == id));    
        }

        public IActionResult Comments()
        {
            return View();
        }

        public IActionResult AddToCart(int id)
        {
            GetCart();
            // TODO include images
            _cart.Add(_db.Products.FirstOrDefault(m=>m.ProductId == id));
            SaveCart();
            return RedirectToAction("Products");
        }

        public IActionResult ViewCart()
        {
            GetCart();
            return View();
        }

        [NonAction]
        public void SaveCart()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, _cart);
                string cart = Convert.ToBase64String(ms.ToArray());
                HttpContext.Session.SetString("ShoppingCart", cart);
            }
        }

        [NonAction]
        public void GetCart()
        {
            string cart = HttpContext.Session.GetString("ShoppingCart");
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
            ViewBag.Cart = _cart;
        }
    }
}
