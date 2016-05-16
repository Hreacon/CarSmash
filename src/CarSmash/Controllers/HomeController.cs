using System;
using System.Collections.Generic;
using System.Linq;
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
            var products = await _db.Products.Include(m=>m.Images).ToListAsync();
            return View(products);
        }

        public IActionResult ProductDetails(int id)
        {
            return View("Details", _db.Products.FirstOrDefault(m=>m.ProductId == id));    
        }


        public IActionResult Details(int id)
        {
            var thisProduct = _db.Products.FirstOrDefault(p => p.ProductId == id);
            return View(thisProduct);
        }

        public IActionResult Comments()
        {
            return View();
        }
    }
}
