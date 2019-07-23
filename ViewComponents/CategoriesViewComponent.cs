using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CategoriesViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await (from t in _context.Tag
                               select new CategoryComponentVm
                               {
                                   Id = t.Id,
                                   Name = t.Name,
                                   Count = t.ArticleTag.Count
                               }).ToListAsync();
            return View(model);
        }
    }
}
