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
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Data.Entity;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

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
       
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            GetCart();
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
            return View("Details", GetProduct(id));    
        }

        public IActionResult Comments()
        {
            return View();
        }

        public IActionResult AddToCart(int id)
        {
            // TODO include images
            _cart.Add(GetProduct(id));
            SaveCart();
            return RedirectToAction("Products");
        }

        public IActionResult RemoveFromCart(int id)
        {
            _cart.Remove(GetProduct(id));
            SaveCart();
            return RedirectToAction("ViewCart");
        }
        public IActionResult ViewCart()
        {
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

        public IActionResult Test(string source)
        {
            // view shopping cart
            // checkout button
            // input payment stuff... leave fake options in so people dont put in card
            //show success
            // show our account balance somewhere
            RestClient client = new RestClient("https://api.stripe.com/v1");
            client.Authenticator = new HttpBasicAuthenticator("sk_test_VSvoTXCfc6VeAVz6YGdRBiKu:", "");
            RestRequest request = new RestRequest("/charges");
            request.AddParameter("amount", 10000);
            request.AddParameter("currency", "usd");
            request.Method = Method.POST;
            request.AddParameter("source", source);

            var response = client.Execute(request);

            return Content(response.Content);
        }

        public IActionResult Checkout()
        {
       
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(FormCollection call)
        {
            var source = Request.Form["stripeToken"];

            RestClient client = new RestClient("https://api.stripe.com/v1");
            client.Authenticator = new HttpBasicAuthenticator("sk_test_VSvoTXCfc6VeAVz6YGdRBiKu:", "");
            RestRequest request = new RestRequest("/charges");

            request.AddParameter("amount", Math.Floor(_cart.Total * 100));

            request.AddParameter("currency", "usd");
            request.Method = Method.POST;
            request.AddParameter("source", source);

            var response = client.Execute(request);

            if (response.Content.Contains("amount\": "))
            {
                //successful transaction
                
                var chargeId = response.Content.Substring(response.Content.IndexOf(":")+4, response.Content.IndexOf(",")-response.Content.IndexOf(":")-4);
                _db.Orders.Add(new Order()
                {
                    Total = _cart.Total,
                    Status = 1,
                    stripeResponseJson = response.Content,
                    stripeChargeId = chargeId,
                    Message = "None"
                });
                _db.SaveChanges();
                var order = _db.Orders.FirstOrDefault(m => m.stripeChargeId == chargeId);
                foreach (var product in _cart.Products)
                {
                    _db.OrderProducts.Add(new OrderProduct()
                    {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId
                    });
                }
                _db.SaveChanges();
                return View("OrderSuccessful");
            }
            return View("Index");
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

        [NonAction]
        public Product GetProduct(int id)
        {
            return _db.Products.Include(m => m.Images).FirstOrDefault(m => m.ProductId == id);
        }


    }
}
