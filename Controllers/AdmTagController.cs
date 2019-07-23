using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class AdmTagController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdmTagController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Tag.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tag model)
        {
            if (ModelState.IsValid)
            {
                _context.Tag.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var model = _context.Tag.FirstOrDefault(x => x.Id == id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Tag model)
        {
            if (ModelState.IsValid)
            {
                _context.Tag.Update(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var model = _context.Tag.FirstOrDefault(x => x.Id == id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(Tag model)
        {
            if (ModelState.IsValid)
            {
                _context.Tag.Remove(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

    }
}