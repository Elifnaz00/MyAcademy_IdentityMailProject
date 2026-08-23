using IdentityMail.Web.Context;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.Controllers
{
    public class CategoryController(AppDbContext _context) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories= await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category= await _context.Categories.FindAsync(id);
            return View(category);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            _context.Categories.Update(category); 
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category= await _context.Categories.FindAsync(id);
           
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            
            return View();
        }

    }
}
