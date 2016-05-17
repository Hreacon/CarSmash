using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Mvc;
using CarSmash.Models;
using Microsoft.Data.Entity;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

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

        public IActionResult Test()
        {
            RestClient client = new RestClient("https://api.stripe.com/v1");
            client.Authenticator = new HttpBasicAuthenticator("sk_test_VSvoTXCfc6VeAVz6YGdRBiKu:", "");
            RestRequest request = new RestRequest("/balance");
            //request.AddParameter("amount", 100);
            //request.AddParameter("currency", "usd");
            //request.Method = Method.POST;
            //Dictionary<string, string> card = new Dictionary<string, string>
            //{
            //    ["object"] = "card",
            //    ["exp_month"] = "05",
            //    ["exp_year"] = "2017",
            //    ["number"] = "4000 0000 0000 0077",
            //    ["cvc"] = "999"
            //};
            //var serialCard = JsonConvert.SerializeObject(card);
            //request.AddParameter("source", serialCard);

            var response = client.Execute(request);

            return Content(response.Content);
        }
    }
}
