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
    public class BiographyViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public BiographyViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("~/artikull-{id}/{serp}")]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model1 = _context.ArticleTag.OrderBy(a => a.Tag).Where(x => x.Tag.Name == "Biografi").FirstOrDefault();
            var model = await (from x in _context.Article.Where(x => x.Id == model1.ArticleId)
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
                                   Title = x.Title
                               }).OrderBy(a => a.DatePublished)
                               .ToListAsync();
            return View(model);
        }
    }
}
