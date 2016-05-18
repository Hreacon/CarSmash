using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using CarSmash.Models;
using Microsoft.AspNet.Authorization;

namespace CarSmash.Controllers
{
    [Authorize(Roles="Admin")]
    public class ImagesController : CarSmashController
    {
        private ApplicationDbContext _context;

        public ImagesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Images
        public IActionResult Index()
        {
            return View(_context.Images.ToList());
        }

        // GET: Images/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Image image = _context.Images.Single(m => m.ImageId == id);
            if (image == null)
            {
                return HttpNotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Images/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Image image)
        {
            if (ModelState.IsValid)
            {
                _context.Images.Add(image);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(image);
        }

        // GET: Images/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Image image = _context.Images.Single(m => m.ImageId == id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Images/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Image image)
        {
            if (ModelState.IsValid)
            {
                _context.Update(image);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(image);
        }

        // GET: Images/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Image image = _context.Images.Single(m => m.ImageId == id);
            if (image == null)
            {
                return HttpNotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Image image = _context.Images.Single(m => m.ImageId == id);
            _context.Images.Remove(image);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
