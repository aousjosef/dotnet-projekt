using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fastigheterse.Data;
using Fastigheterse.Models;

namespace Fastigheterse.Controllers
{
    public class PropertyCatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyCatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PropertyCats
        public async Task<IActionResult> Index()
        {
            return View(await _context.PropertyCats.ToListAsync());
        }

        // GET: PropertyCats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyCat = await _context.PropertyCats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (propertyCat == null)
            {
                return NotFound();
            }

            return View(propertyCat);
        }

        // GET: PropertyCats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PropertyCats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] PropertyCat propertyCat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(propertyCat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(propertyCat);
        }

        // GET: PropertyCats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyCat = await _context.PropertyCats.FindAsync(id);
            if (propertyCat == null)
            {
                return NotFound();
            }
            return View(propertyCat);
        }

        // POST: PropertyCats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] PropertyCat propertyCat)
        {
            if (id != propertyCat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyCat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyCatExists(propertyCat.Id))
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
            return View(propertyCat);
        }

        // GET: PropertyCats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyCat = await _context.PropertyCats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (propertyCat == null)
            {
                return NotFound();
            }

            return View(propertyCat);
        }

        // POST: PropertyCats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyCat = await _context.PropertyCats.FindAsync(id);
            if (propertyCat != null)
            {
                _context.PropertyCats.Remove(propertyCat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyCatExists(int id)
        {
            return _context.PropertyCats.Any(e => e.Id == id);
        }
    }
}
