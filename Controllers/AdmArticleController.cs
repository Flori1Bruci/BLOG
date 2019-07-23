using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Entity;
using Blog.Helpers;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{

    public class AdmArticleController : Controller
    {
        public const int pagesize = 20;
        private readonly ApplicationDbContext _context;
        private readonly IDocumentsService _documents;
        public AdmArticleController(ApplicationDbContext context, IDocumentsService documents)
        {
            _context = context;
            _documents = documents;
        }

        [Authorize(Roles = AppRoles.WORKER)]
        [Route("~/staff")]
        public IActionResult Index(int? pageNumber)
        {
            if (!pageNumber.HasValue || pageNumber < 1)
            {
                pageNumber = 1;
            }
            var query = _context.Article.Include(x => x.Worker).AsQueryable();
            var totalcount = query.Count();
            var items = query.OrderBy(x => x.Id)
                .Skip((pageNumber.Value - 1) * pagesize)
            .Take(pagesize)
            .ToList();

            var pgmodel = new PagedList<Article>
            {
                CurrentPage = pageNumber.Value,
                Items = items,
                PageSize = pagesize,
                TotalCount = totalcount
            };

            return View(pgmodel);
        }
        //[Authorize(Roles = AppRoles.WORKER)]
        public IActionResult Create()
        {
            var tag = _context.Tag.Select(x => new
            {
                x.Id,
                x.Name
            });
            ViewBag.Taget = new SelectList(tag, "Id", "Name");
            ViewBag.Status = new SelectList(EnumHelper.StatusList(), "Id", "Text");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ArticleCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                var tag = _context.Tag.Select(x => new
                {
                    x.Id,
                    x.Name
                });
                ViewBag.Taget = new SelectList(tag, "Id", "Name");
                ViewBag.Status = new SelectList(EnumHelper.StatusList(), "Id", "Text");
                return View(model);
            }

            var dbmodel = new Article
            {
                Id = model.Id,
                Title = model.Title,
                Serp = SERP.GetFriendlyTitle(model.Title),
                Body = model.Body,
                Meta = model.Meta,
                DateCreated = DateTime.UtcNow,
                DateModifed = null,
                Status = model.Status,
                WorkerId = User.GetWorkerId(),
            };
            if (dbmodel.Status == Status.Published)
            {
                dbmodel.DatePublished = DateTime.UtcNow;
            }
            else
            {
                dbmodel.DatePublished = null;
            }

            _context.Article.Add(dbmodel);
            if (model.ArticleTag != null)
            {
                foreach (var item in model.ArticleTag)
                {
                    _context.ArticleTag.Add(new ArticleTag
                    {
                        ArticleId = dbmodel.Id,
                        TagId = item

                    });
                }
            }


            if (model.Photo != null && model.Photo.Count > 0)
            {
                var documentResult = await _documents.SaveArticleImages(model.Photo);
                foreach (var item in documentResult)
                {
                    if (item.Item1 != "" && item.Item2 != "")
                    {
                        _context.Photo.Add(new Photo
                        {
                            MimeType = item.Item1,
                            Name = item.Item2,
                            ArticleId = dbmodel.Id
                        });
                    }
                }
            }


            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var tag = _context.Tag.Select(x => new
            {
                x.Id,
                x.Name
            });
            ViewBag.Taget = new SelectList(tag, "Id", "Name");
            ViewBag.Status = new SelectList(EnumHelper.StatusList(), "Id", "Text");

            var model = _context.Article.Where(x => x.Id == id).Select(x => new ArticleCreateVM
            {
                Body = x.Body,
                DateCreated = x.DateCreated,
                DateModifed = x.DateModifed,
                DatePublished = x.DatePublished,
                Meta = x.Meta,
                Serp = x.Serp,
                Status = x.Status,
                Title = x.Title,
                WorkerId = x.WorkerId,
                ArticleTag = x.ArticleTag.Select(y => y.TagId).ToList()

            }).SingleOrDefault();
            //model.ArticleTag = _context.ArticleTag.Select(x => x.TagId).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ArticleCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var dbmodel = _context.Article.Where(x => x.Id == model.Id).FirstOrDefault();
                if (dbmodel == null)
                    return NotFound();

                if (model.Photo != null && model.Photo.Count > 0)
                {
                    var documentResult = await _documents.SaveArticleImages(model.Photo);
                    foreach (var item in documentResult)
                    {
                        if (item.Item1 != "" && item.Item2 != "")
                        {
                            _context.Photo.Add(new Photo
                            {
                                MimeType = item.Item1,
                                Name = item.Item2,
                                ArticleId = dbmodel.Id
                            });
                        }
                    }
                }

                var oldArticleTag = _context.ArticleTag.Where(x => x.ArticleId == dbmodel.Id).ToList();

                _context.RemoveRange(oldArticleTag);

                foreach (var item in model.ArticleTag)
                {
                    _context.ArticleTag.Add(new ArticleTag
                    {
                        ArticleId = dbmodel.Id,
                        TagId = item
                    });
                }

                dbmodel.Title = model.Title;
                dbmodel.Status = model.Status;
                dbmodel.Serp = SERP.GetFriendlyTitle(model.Title);
                dbmodel.Body = model.Body;
                _context.Entry(dbmodel).Property(x => x.DateCreated).IsModified = false;
                dbmodel.DateModifed = DateTime.UtcNow;
                dbmodel.Meta = model.Meta;
                dbmodel.WorkerId = User.GetWorkerId();
                if (dbmodel.Status == Status.Published)
                {
                    dbmodel.DatePublished = DateTime.UtcNow;
                }
                else
                {
                    dbmodel.DatePublished = null;
                }
                _context.Entry(dbmodel).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
                return NotFound();


        }

        [Route("~/artikull-{id}/{serp}")]
        public IActionResult Details(int id, string serp)
        {
            if (id == 0)
                return NotFound();

            var dbmodel = _context.Article
                .Include(x => x.Worker)
                .Include(x => x.Photo)
                .SingleOrDefault(x => x.Id == id);


            var model = new ArticleDetailsVM
            {
                Id = dbmodel.Id,
                Body = dbmodel.Body,
                DateCreated = dbmodel.DateCreated,
                DateModifed = dbmodel.DateModifed,
                DatePublished = dbmodel.DatePublished,
                ImageName = dbmodel.Photo.Select(x => x.Name).ToList(),
                Meta = dbmodel.Meta,
                Serp = dbmodel.Serp,
                Status = dbmodel.Status.ToString(),
                Title = dbmodel.Title,
                Worker = dbmodel.Worker.FirstName
            };


            if (model == null)
                return NotFound();


            model.Tag = _context.ArticleTag
                .Where(x => x.ArticleId == model.Id)
                .Select(x => x.Tag.Name).ToList();


            return View(model);
        }

        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost]
        public IActionResult Delete(Article model)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            _context.Article.Remove(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}