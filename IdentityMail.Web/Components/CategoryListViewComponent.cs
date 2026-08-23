using IdentityMail.Web.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.Components
{
    public class CategoryListViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;

        public CategoryListViewComponent(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categoryList= await _dbContext.Categories.AsNoTracking().ToListAsync();
            var count= categoryList.Count;
            ViewBag.CategoryCount= count;
            return View(categoryList);
        }
    }
}
