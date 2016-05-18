using CarSmash.Models;
using CarSmash.ViewModels.Products;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarSmash.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;

        public ProductsController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Include(p=>p.Images).ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = await _context.Products.Include(m => m.Images).SingleAsync(m => m.ProductId == id);
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
        public async Task<IActionResult> Create(Product product, ICollection<IFormFile> files = null)
        {
            if (ModelState.IsValid)
            {
                if (files != null)
                {
                    var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                    foreach (var file in files)
                    {
                        var image = new Image();
                        if (file.Length > 0)
                        {
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            var thePath = Path.Combine(uploads, fileName);
                            await file.SaveAsAsync(thePath);

                            image.Url = "~/uploads/" + fileName;
                            image.Product = product;
                            _context.Images.Add(image);
                        }
                    }
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return View("Details", product);
            } 
            return View();
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = await _context.Products.Include(i => i.Images).SingleAsync(p => p.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            product.Images = await _context.Images.Include(i=>i.Product).Where(i=>i.Product == product).ToListAsync();
            if (ModelState.IsValid)
            {
                string formFieldId = "image.";

                foreach (var key in Request.Form.Keys)
                {
                    if (key.Contains(formFieldId))
                    {
                        var imageId = int.Parse(key.Substring(6));

                        foreach (var image in product.Images)
                        {
                            if (image.ImageId == imageId)
                            {
                                image.Url = Request.Form[key];
                                _context.Update(image);
                            }
                        }
                    }
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