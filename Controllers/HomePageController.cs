using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Entity;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class HomePageController : Controller
    {
        public const int pagesize = 6;
        private readonly ApplicationDbContext _context;
        private readonly IDocumentsService _documents;

        public HomePageController(ApplicationDbContext context, IDocumentsService documents)
        {
            _context = context;
            _documents = documents;
        }

        [Route("~/")]
        [Route("~/kategoria/{tag}")]
        public IActionResult Index(int? pageNumber, string tag, string search)
        {
            if (string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(search))
            {
                if (!pageNumber.HasValue || pageNumber < 1)
                {
                    pageNumber = 1;
                }

                var query = _context.Article.Where(a => a.Status == Helpers.Status.Published).Include(x => x.Photo).AsQueryable();
                var totalcount = query.Count();
                var items = (from x in query
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

                             }).OrderByDescending(x => x.DatePublished)
                    .Skip((pageNumber.Value - 1) * pagesize)
                .Take(pagesize)
                .ToList();

                var pgmodel = new PagedList<ArticleHomepageVM>
                {
                    CurrentPage = pageNumber.Value,
                    Items = items,
                    PageSize = pagesize,
                    TotalCount = totalcount
                };

                return View(pgmodel);

            }
            else if (string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(tag))
            {
                if (!pageNumber.HasValue || pageNumber < 1)
                {
                    pageNumber = 1;
                }

                //#region test
                //var model = (from a in _context.Article
                //             where a.ArticleTag.Any(at => at.TagId == tag))
                //#endregion

                // marrim tagun me ket name
                var dbTag = _context.Tag.Where(x => x.Name.ToLower() == tag.ToLower()).FirstOrDefault();

                if (dbTag == null)
                    return NotFound();
                // marrim listen e artikujve me ket tag
                var model = (from a in _context.Article
                             where a.ArticleTag.Any(at => at.TagId == dbTag.Id)
                             select a);


                var totalCount = model.Count();
                var items = model.Where(a => a.Status == Helpers.Status.Published).Select(x => new ArticleHomepageVM
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
                }).OrderByDescending(x => x.DatePublished)
                .Skip((pageNumber.Value - 1) * pagesize)
                .Take(pagesize)
                .ToList();

                var pgmodel = new PagedList<ArticleHomepageVM>
                {
                    CurrentPage = pageNumber.Value,
                    Items = items,
                    PageSize = pagesize,
                    TotalCount = totalCount
                };
                return View(pgmodel);
            }
            else if (string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(search))
            {
                if (!pageNumber.HasValue || pageNumber < 1)
                {
                    pageNumber = 1;
                }

                var tagetSipasSearch = _context.Tag
                    .Where(x => x.Name.Contains(search)).Select(x => x.Id).ToList();

                var model = (from a in _context.Article.Include(p => p.Photo)
                             where a.Meta.Contains(search)
                                   || a.Title.Contains(search)
                                   || a.ArticleTag.Any(at => tagetSipasSearch.Contains(at.TagId))
                             select a);




                var totalCount = model.Count();
                var items = model.Select(x => new ArticleHomepageVM
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
                }).OrderByDescending(x => x.DatePublished)
                .Skip((pageNumber.Value - 1) * pagesize)
                .Take(pagesize)
                .ToList();

                var pgmodel = new PagedList<ArticleHomepageVM>
                {
                    CurrentPage = pageNumber.Value,
                    Items = items,
                    PageSize = pagesize,
                    TotalCount = totalCount
                };
                return View(pgmodel);

            }
            else
            {
                if (!pageNumber.HasValue || pageNumber < 1)
                {
                    pageNumber = 1;
                }

                var model1 = _context.ArticleTag
                    .Where(b => b.Tag.Name.ToLower().Contains(search))
                    .Select(a => a.ArticleId).ToList();

                var dbTag = _context.Tag.Where(x => x.Name.ToLower() == tag.ToLower()).FirstOrDefault();
                if (dbTag == null)
                    return NotFound();
                var model = _context.Article
                    .Include(x => x.Photo)
                    .Include(a => a.ArticleTag)
                    .Where(y => y.Meta.ToLower().Contains(search.ToLower()) ||
                    y.Title.ToLower().Contains(search.ToLower()) ||
                    y.Id == model1.FirstOrDefault()
                    && y.ArticleTag.Any(at => at.TagId == dbTag.Id)).AsQueryable();

                var totalCount = model.Count();
                var items = model.Select(x => new ArticleHomepageVM
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
                }).OrderByDescending(x => x.DatePublished)
                .Skip((pageNumber.Value - 1) * pagesize)
                .Take(pagesize)
                .ToList();

                var pgmodel = new PagedList<ArticleHomepageVM>
                {
                    CurrentPage = pageNumber.Value,
                    Items = items,
                    PageSize = pagesize,
                    TotalCount = totalCount
                };
                return View(pgmodel);



            }
        }

        //[Route("~/tag-{urlFragment}")]
        //public IActionResult Tag(int? pageNumber, string urlFragment, string search)
        //{
        //    return View();
        //}
    }
}