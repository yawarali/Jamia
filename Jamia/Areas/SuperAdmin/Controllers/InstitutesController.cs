﻿using Jamia.Data;
using Jamia.Infrastructure;
using Jamia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jamia.Areas.SuperAdmin.Controllers
{
    [Area(AreaNames.SuperAdmin)]
    public class InstitutesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public InstitutesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: SuperAdmin/Institutes
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(await _context.Institute.Where(ins => _context.UserInstitute.Any(userIns => userIns.InstituteId == ins.ID && userIns.ApplicationUserId == user.Id)).ToListAsync());
        }

        // GET: SuperAdmin/Institutes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var institute = await _context.Institute
                .FirstOrDefaultAsync(m => m.ID == id);
            if (institute == null)
            {
                return NotFound();
            }

            return View(institute);
        }

        // GET: SuperAdmin/Institutes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SuperAdmin/Institutes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description")] Institute institute)
        {
            if (ModelState.IsValid)
            {
                institute.ID = Guid.NewGuid();
                _context.Add(institute);
                var user = await _userManager.GetUserAsync(User);
                _context.UserInstitute.Add(new UserInstitute { ApplicationUser = user, ApplicationUserId = user.Id, Institute = institute, InstituteId = institute.ID });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(institute);
        }

        // GET: SuperAdmin/Institutes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var institute = await _context.Institute.FindAsync(id);
            if (institute == null)
            {
                return NotFound();
            }
            return View(institute);
        }

        // POST: SuperAdmin/Institutes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,Name,Description")] Institute institute)
        {
            if (id != institute.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(institute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstituteExists(institute.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(institute);
        }

        // GET: SuperAdmin/Institutes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var institute = await _context.Institute
                .FirstOrDefaultAsync(m => m.ID == id);
            if (institute == null)
            {
                return NotFound();
            }

            return View(institute);
        }

        // POST: SuperAdmin/Institutes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var institute = await _context.Institute.FindAsync(id);
            _context.Institute.Remove(institute);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstituteExists(Guid id)
        {
            return _context.Institute.Any(e => e.ID == id);
        }

        [HttpGet]
        public IActionResult GetInstitute(string term)
        {
            var result = _context.Institute.Where(x => x.Name.Contains(term)).Select(x => x.Name).ToList();
            return Json(result);
        }
    }
}
