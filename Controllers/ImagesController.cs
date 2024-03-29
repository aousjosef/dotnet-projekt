﻿using Fastigheterse.Data;
using Fastigheterse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fastigheterse.Controllers
{
    [Authorize]
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ImagesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Images.Include(i => i.Property);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Property)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        //Enabled get with Id so when image gets edited from inside a property, it automatilcy selects it. If Id is sent as a parameter.
        [HttpGet("Images/Create/{propertyId?}")]

        public IActionResult Create(int? propertyId)
        {
            if (propertyId.HasValue)
            {
                ViewBag.PropertyId = new SelectList(_context.Properties, "Id", "Title", propertyId);
            }
            else
            {
                ViewBag.PropertyId = new SelectList(_context.Properties, "Id", "Title");
            }

            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Url,PropertyId")] Image imageModel, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    foreach (var file in imageFiles)
                    {
                        var filePath = await SaveFileAsync(file); // Save each file and get back the file path
                        var image = new Image
                        {
                            Url = filePath,
                            PropertyId = imageModel.PropertyId // Assuming PropertyId is what links the image to a property

                        };
                        _context.Add(image);
                    }
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                }
            }
            ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Title", imageModel.PropertyId);

            //retunerar till edit page med specifik property
            return RedirectToAction("Edit", "Properties", new { id = imageModel.PropertyId });

        }


        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Title", image.PropertyId);
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Url,PropertyId")] Image image)
        {
            if (id != image.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.Id))
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
            ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Title", image.PropertyId);
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Property)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Url.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            //creates refersurl which the url the request was sent from, then if it exits, we get sent back there.

            string refererUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(refererUrl))
            {
                return Redirect(refererUrl);
            }
            else
            {
                // Fallback if Referer is not available
                return RedirectToAction(nameof(Index)); // Adjust as needed
            }
        }

        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.Id == id);
        }


        private async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Define the directory path to save images
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/media/images");

            // Ensure the directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Create a unique file name to prevent overwriting existing files
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return the relative path as stored in the database
            return Path.Combine("media/images", fileName); // Returns a relative path to be stored in the database
        }
    }
}
