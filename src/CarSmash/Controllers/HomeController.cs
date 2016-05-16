using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using CarSmash.Models;
using Microsoft.Data.Entity;

namespace CarSmash.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _db { get; set; }

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
       
            public IActionResult Index()
        {
            
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

            return View(await _db.Products.ToListAsync());
        }

        public IActionResult Comments()
        {
            return View();
        }
    }
}
