using Blog.Data;
using Blog.Helpers;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.ViewComponents
{
    public class ArticleCategoryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public ArticleCategoryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            var vm = new ArticleCategoryViewModel { };
            // get categ
            var dbCategory = await _context.Tag.Where(x => x.Id == id).FirstOrDefaultAsync();
            // set text
            vm.TagId = id.Value;
            vm.DisplayTitle = dbCategory.Name;
            // get articles
            vm.Articles = await (from a in _context.Article
                                 where a.Status == Status.Published
                                       && a.ArticleTag.Any(x => x.TagId == id)
                                 orderby a.DatePublished descending
                                 select new ArticleBaseModel
                                 {
                                     Id = a.Id,
                                     Title = a.Title,
                                     Serp = a.Serp,
                                     PublishedDate = a.DatePublished,
                                     ImageName = a.Photo.Select(x => x.Name).FirstOrDefault()
                                 })
                                    .Take(6)
                                    .ToListAsync();

            #region del
            //var modeltag = _context.ArticleTag
            //    .Where(x => x.ArticleId == id)
            //    .Select(x => x.TagId)
            //    .ToList();


            //var lista = _context.Article
            //    .Include(x => x.ArticleTag)
            //    .Where(a => a.ArticleTag.Select(y => y.TagId).Any(b => modeltag.Contains(b)));

            //var listmodelesh = await (from x in lista.Where(a => a.Id != id).Where(b => b.Status == Status.Published)
            //                          select new ArticleHomepageVM
            //                          {
            //                              Id = x.Id,
            //                              Body = x.Body,
            //                              DateCreated = x.DateCreated,
            //                              DateModifed = x.DateModifed,
            //                              DatePublished = x.DatePublished,
            //                              ImageName = x.Photo.Select(y => y.Name).FirstOrDefault(),
            //                              Meta = x.Meta,
            //                              Serp = x.Serp,
            //                              Status = x.Status.ToString(),
            //                              Title = x.Title,
            //                              Worker = x.Worker.FirstName,
            //                              Tag = x.ArticleTag.Select(y => y.Tag.Name).Take(2).ToList()
            //                          }).ToListAsync(); 
            #endregion
            return View(vm);
        }
    }
}
