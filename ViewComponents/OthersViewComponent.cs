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
    public class OthersViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public OthersViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            

            var listmodelesh = await (from x in _context.Article.Where(a => a.Status == Helpers.Status.Published)
                                      select new ArticleHomepageVM
                                      {
                                          Id = x.Id,
                                          Body = x.Body,
                                          DateCreated = x.DateCreated,
                                          DateModifed = x.DateModifed,
                                          DatePublished = x.DatePublished,
                                          ImageName = x.Photo.Select(y => y.Name).FirstOrDefault(),
                                          Meta = x.Meta,
                                          Serp = x.Serp,
                                          Status = x.Status.ToString(),
                                          Title = x.Title,
                                          Worker = x.Worker.FirstName,
                                          Tag = x.ArticleTag.Select(y => y.Tag.Name).Take(2).ToList()
                                      }).OrderBy(b=> b.DatePublished).ToListAsync();
            return View(listmodelesh);
        }
    }
}
