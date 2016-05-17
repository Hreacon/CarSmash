using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using CarSmash.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using System.IO;
using System.Text.RegularExpressions;
using CarSmash.ViewModels.Products;

namespace CarSmash.Controllers
{
    [Authorize(Roles="Admin")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = await _context.Products.SingleAsync(m => m.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProduct product)
        {      
            if (ModelState.IsValid)
            {
                //_context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", product);
            }
            return View(product);
        }

        [NonAction]
        public Product SavePhoto(Product product, IFormFile file)
        {
            Image newImage = new Image();
            newImage.Url = Path.Combine("images/products", Regex.Replace(product.Name, " ", "") + product.ProductId + ".jpg");
            
            file.SaveAs(newImage.Url);
            newImage.Url = "~/" + newImage.Url;
            product.Images.Add(newImage);
            return product;
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = await _context.Products.SingleAsync(m => m.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product,  IFormFile file = null)
        {

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    product = this.SavePhoto(product, file);
                }
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = await _context.Products.SingleAsync(m => m.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product product = await _context.Products.SingleAsync(m => m.ProductId == id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
