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
    public class CarouselFrontViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CarouselFrontViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await (from x in _context.Article.Where(c => c.Status == Helpers.Status.Published && c.Id != 72).Include(a => a.Photo).Include(b => b.ArticleTag)
                               select new ArticleHomepageVM
                               {
                                   Id = x.Id,
                                   Body = x.Body,
                                   DateCreated = x.DateCreated,
                                   DateModifed = x.DateModifed,
                                   DatePublished = x.DatePublished.Value.Date,
                                   ImageName = x.Photo.Select(y => y.Name).FirstOrDefault(),
                                   Meta = x.Meta,
                                   Serp = x.Serp,
                                   Status = x.Status.ToString(),
                                   Tag = x.ArticleTag.Where(a => a.ArticleId == x.Id).Select(z => z.Tag.Name).ToList(),
                                   Title = x.Title
                               }).OrderBy(y => y.DatePublished)
                               .ToListAsync();
            return View(model);
        }
    }
}
