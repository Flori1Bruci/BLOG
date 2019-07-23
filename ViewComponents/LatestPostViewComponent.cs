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

    public class LatestPostViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public LatestPostViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await (from x in _context.Article
                               where  x.Status == Helpers.Status.Published
                               orderby x.DatePublished
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
                               }).ToListAsync();
            return View(model);
        }
    }
}
