using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarSmash.Models;
using CarSmash.Services;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

namespace CarSmash.Controllers
{
    public class CarSmashController : Controller
    {
        public ShoppingCart _cart;
        // OnActionExecuting runs on after the constructor, before the action/route/method
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            // could not put this method in the constructor because HttpContext is not available in the constructor
            _cart = CartService.GetCart(HttpContext);
            ViewBag.Cart = _cart;
        }
    }
}
