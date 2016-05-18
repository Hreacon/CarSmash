using CarSmash.Models;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Data.Entity;
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
        // OnActionExecuting runs on after the constructor, before the action/route/method
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            // could not put this method in the constructor because HttpContext is not available in the constructor
            GetCart();
        }

        public IActionResult Index(string ajax)
        {
            //if (!_db.Products.Any())
            //{
            //    // if no products create a test product
            //    //TODO remove on deploy
            //    _db.Images.Add(new Image()
            //    {
            //        Url = "/images/products/nurburg.jpg"
            //    });
            //    _db.SaveChanges();
            //    _db.Products.Add(new Product()
            //    {
            //        Description = "Nurburgring Sticker",
            //        Images = _db.Images.ToList(),
            //        Name = "Nurburg Sticker",
            //        Price = 10.99
            //    });
            //    _db.SaveChanges();
            //}
            if(ajax == "true") return PartialView();
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

        public async Task<IActionResult> Products(string ajax)
        {
            var products = await _db.Products.Include(m => m.Images).ToListAsync();
            if (ajax == "true") return PartialView("Products", products);
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

        public IActionResult AddToCart(int id, string ajax)
        {
            // TODO include images
            _cart.Add(GetProduct(id));
            SaveCart();
            return RedirectToAction("Products", new {ajax = ajax});
        }

        public IActionResult RemoveFromCart(int id, string ajax)
        {
            _cart.Remove(GetProduct(id));
            SaveCart();
            //for redirecttoaction make new generic object and pass it in 
            return RedirectToAction("ViewCart", new { ajax = ajax});
        }

        [HttpPost]
        public IActionResult UpdateCart(FormCollection col)
        {
            string formFieldId = "quantity.";
            var products = new List<Product>();
            products.AddRange(_cart.Products);
            foreach (var key in Request.Form.Keys)
            {
                if (key.Contains(formFieldId))
                {
                    foreach (var product in products)
                    {
                        if (key == formFieldId + product.ProductId)
                        {
                            product.Quantity = int.Parse(Request.Form[key]);
                            if (product.Quantity == 0)
                            {
                                _cart.Products.Remove(product);
                            }
                        }
                    }
                }
            }
            SaveCart();
            return RedirectToAction("ViewCart");
        }

        public IActionResult ViewCart(string ajax)
        {
            if (ajax == "true") return PartialView("ViewCart");
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

        public IActionResult Checkout(string ajax)
        {
            if (ajax == "true") return PartialView("Checkout");
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
                _cart = new ShoppingCart();
                SaveCart();
                return View("OrderSuccessful");
            }
            return View("Index");
        }

        [NonAction] // not a route
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

        public IActionResult Videos(string ajax)
        {
            //partial view returns view without _layout
            if (ajax == "true") return PartialView("Videos");
            return View();
        }
    }
}
