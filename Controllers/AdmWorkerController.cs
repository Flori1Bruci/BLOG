using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Entity;
using Blog.Helpers;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class AdmWorkerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdmWorkerController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(_context.Worker.ToList());
        }

        public IActionResult Register()
        {
            ViewBag.Body = new SelectList(EnumHelper.PositionList(), "Id", "Text");
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Register(WorkerVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    return View(model);
                }

                var aspNetUser = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(aspNetUser, model.Password);
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(aspNetUser, AppRoles.WORKER);

                var dbWorker = new Worker
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Age = DateTime.UtcNow.Year - model.Birthday.Year,
                    Joined = model.Joined,
                    Position = (Position)model.Position,
                    UserGuid = aspNetUser.Id,
                    Email = model.Email,
                    Mobile = model.Mobile
                };
                _context.Worker.Add(dbWorker);
                _context.SaveChanges();

                var res = await _userManager.AddClaimAsync(aspNetUser, new System.Security.Claims.Claim(AppClaims.WORKER_ID, dbWorker.Id.ToString()));
                if (!res.Succeeded)
                {
                    // TODO: ...
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Body = new SelectList(EnumHelper.PositionList(), "Id", "Text");
            return View();


        }

        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            ViewBag.Body = new SelectList(EnumHelper.PositionList(), "Id", "Text");
            var model = _context.Worker.FirstOrDefault(x => x.Id == id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Worker model)
        {
            if (ModelState.IsValid)
            {
                _context.Worker.Update(model);
                _context.Entry(model).Property(x => x.UserGuid).IsModified = false;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Body = new SelectList(EnumHelper.PositionList(), "Id", "Text");
            return View(model);
        }

        public IActionResult MyProfile(string email)
        {
            var model = _context.Worker.Where(x => x.Email == email).SingleOrDefault();
            return View(model);
        }

    }
}