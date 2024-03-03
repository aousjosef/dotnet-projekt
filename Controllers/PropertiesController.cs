using Fastigheterse.Data;
using Fastigheterse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fastigheterse.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public PropertiesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Properties.Include(p => p.PropertyCat);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .Include(p => p.PropertyCat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@property == null)
            {
                return NotFound();
            }

            //show image on details page, sends data as a viewbag
            var applicationDbContextImages = _context.Images
                                          .Where(i => i.PropertyId == id)
                                          .Include(i => i.Property);

            //included tolistasync. and sent it to editpage
            ViewBag.imageList = await applicationDbContextImages.ToListAsync();

            return View(@property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            ViewData["PropertyCatId"] = new SelectList(_context.PropertyCats, "Id", "Name");
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Size,Description,Price,Rooms,City,Zipcode,PropertyCatId")] Property @property, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {

                property.CreatedDate = DateTime.Now; // Set the current date and time
                property.CreatedBy = User.Identity.Name; // Set the current user's username
                _context.Add(@property);
                await _context.SaveChangesAsync();

                if (imageFiles != null && imageFiles.Count > 0)
                {
                    foreach (var file in imageFiles)
                    {
                        var filePath = await SaveFileAsync(file); // Save each file and get back the file path
                        var image = new Image { Url = filePath, PropertyId = @property.Id }; // Create image with URL as filePath
                        _context.Images.Add(image);
                    }

                    await _context.SaveChangesAsync(); // Save image paths in the database
                }

                return RedirectToAction(nameof(Index));

            }
            ViewData["PropertyCatId"] = new SelectList(_context.PropertyCats, "Id", "Name", @property.PropertyCatId);
            return View(@property);
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties.FindAsync(id);

            if (@property == null)
            {
                return NotFound();
            }
            ViewData["PropertyCatId"] = new SelectList(_context.PropertyCats, "Id", "Name", @property.PropertyCatId);

            // send image list to view



            var applicationDbContextImages = _context.Images
                                           .Where(i => i.PropertyId == id)
                                           .Include(i => i.Property);

            //included tolistasync. and sent it to editpage
            ViewBag.imageList = await applicationDbContextImages.ToListAsync();
            return View(@property);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Size,Description,Price,Rooms,City,Zipcode,PropertyCatId")] Property @property)
        {
            if (id != @property.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                property.CreatedDate = DateTime.Now; // Set the current date and time
                property.CreatedBy = User.Identity.Name; // Set the current user's username

                try
                {
                    _context.Update(@property);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(@property.Id))
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
            ViewData["PropertyCatId"] = new SelectList(_context.PropertyCats, "Id", "Name", @property.PropertyCatId);
            return View(@property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .Include(p => p.PropertyCat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.Properties
                                .Include(p => p.Images) // Make sure to include the images in the query
                                .FirstOrDefaultAsync(p => p.Id == id);

            if (property != null)
            {
                // Delete image files from the file system
                foreach (var image in property.Images)
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Url.Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Now remove the property (and cascade delete the images from the database)
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
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
