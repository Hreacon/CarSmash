using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using RealEstateGame.Models;

namespace RealEstateGame.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View("Index", _context.Roles.ToList());
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FormCollection collection)
        {
            _context.Roles.Add(new IdentityRole(Request.Form["RoleName"]));
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public IActionResult Assign()
        {
            ViewBag.Users = new SelectList(_userManager.Users.ToList());
            return View();
        }
        
        public IActionResult AddToUser(string username, string roleName)
        {
            var user = GetUser(username);
            var thing = _userManager.AddToRoleAsync(user, roleName).Result;
            return GetRoles(username);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetRoles(string username)
        {
            var user = GetUser(username);
            ViewBag.User = user;
            var userRoles = _userManager.GetRolesAsync(user).Result;
            ViewBag.RolesForThisUser = userRoles;
            var preroles = _context.Roles.ToList();
            var roles = new List<string>();
            foreach (var r in preroles)
            {
                var add = true;
                foreach (var userRole in userRoles)
                {
                    if (userRole == r.Name)
                    {
                        add = false;
                    }
                }
                if (add)
                {
                    roles.Add(r.Name);
                }
            }
            ViewBag.Users = new SelectList(_userManager.Users.ToList());
            ViewBag.Roles = roles;
            return View("Assign");
        }
        
        public ApplicationUser GetUser(string username)
        {
            return _userManager.Users.FirstOrDefault(m => m.UserName == username);
        }

        [HttpGet]
        public IActionResult Edit(string roleName)
        {
            ViewData["roleName"] = roleName;
            return View("Edit");
        }
        
        public IActionResult Delete(string roleName)
        {
            var role = _context.Roles.FirstOrDefault(m => m.Name == roleName);
            if (role != null)
            {
                try
                {
                    _context.Roles.Remove(role);
                    _context.SaveChanges();
                }
                catch
                {
                    ViewData["message"] = "Users still in role " + roleName;
                    return Index();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteFromUser(string username, string roleName)
        {
            var thing = _userManager.RemoveFromRoleAsync(GetUser(username), roleName).Result;
            return GetRoles(username);
        }
    }
}