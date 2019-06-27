using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModel;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{[Area("Admin")]
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuAndCategoryViewModel MenuAndCategoryViewModel { get; set; }

  //      public MenuAndCategoryViewModel MenuAndCategoryViewModel { get => menuAndCategoryViewModel; set => menuAndCategoryViewModel = value; }

        public MenuController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            MenuAndCategoryViewModel = new MenuAndCategoryViewModel()
            {
                Category = _db.Category,
                Menu = new Models.Menu()
            };
        }
        public async Task<IActionResult> Index()
        {
            var menu = await _db.Menu.Include(m=>m.Category).Include(m=>m.SubCategory).ToListAsync();
            return View(menu);
        }
        //Get - Create
        public IActionResult Create()
        {
            return View(MenuAndCategoryViewModel);
        }
        //post - Create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> CreatePOST()
        {
            MenuAndCategoryViewModel.Menu.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(MenuAndCategoryViewModel);
            }
            _db.Menu.Add(MenuAndCategoryViewModel.Menu);
            await _db.SaveChangesAsync();
            //work on the image saving section
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = await _db.Menu.FindAsync(MenuAndCategoryViewModel.Menu.Id);
            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);
                using (var fileStream=new FileStream(Path.Combine(uploads, MenuAndCategoryViewModel.Menu.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuAndCategoryViewModel.Menu.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"/images/" + MenuAndCategoryViewModel.Menu.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuAndCategoryViewModel.Menu.Id + ".png";
                //no files has been uploaded
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //Get - Edit
        public async Task<IActionResult> Edit(int ? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuAndCategoryViewModel.Menu = await _db.Menu.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuAndCategoryViewModel.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuAndCategoryViewModel.Menu.CategoryId).ToListAsync();
            if (MenuAndCategoryViewModel.Menu == null)
            {
                return NotFound();
            }
            return View(MenuAndCategoryViewModel);
        }
        //post - Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditPOST(int ? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuAndCategoryViewModel.Menu.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(MenuAndCategoryViewModel);
            }
            _db.Menu.Add(MenuAndCategoryViewModel.Menu);
            await _db.SaveChangesAsync();
            //work on the image saving section
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = await _db.Menu.FindAsync(MenuAndCategoryViewModel.Menu.Id);
            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(uploads, MenuAndCategoryViewModel.Menu.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuAndCategoryViewModel.Menu.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"/images/" + MenuAndCategoryViewModel.Menu.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuAndCategoryViewModel.Menu.Id + ".png";
                //no files has been uploaded
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

