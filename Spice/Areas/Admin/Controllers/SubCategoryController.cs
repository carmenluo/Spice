using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModel;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(s=>s.Category).ToListAsync();
            return View(subCategories);
        }
        //GET
        public async Task<IActionResult> Create()
        {
            SubCategoryandCategoryViewModel model = new SubCategoryandCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(model);
        }
        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SubCategory subcategory = await _db.SubCategory.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }
            SubCategoryandCategoryViewModel model = new SubCategoryandCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subcategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(model);
        }
        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SubCategoryandCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);
                if (doesSubCategoryExists.Count() > 0)
                {
                    StatusMessage = "Error: Sub Category already exists under category: " + doesSubCategoryExists.First().Category.Name;
                }
                else
                {
                    var subCatFromDb = await _db.SubCategory.FindAsync(id);
                    subCatFromDb.Name = model.SubCategory.Name;
                    subCatFromDb.CategoryId = model.SubCategory.CategoryId;
                    
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            }
            SubCategoryandCategoryViewModel _model = new SubCategoryandCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                ErrorMessage = StatusMessage
            };
            return View(_model);

        }
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryandCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);
                if (doesSubCategoryExists.Count() > 0)
                {
                    StatusMessage = "Error: Sub Category already exists under Category: " + doesSubCategoryExists.First().Category.Name;
                }
                else
                {
                    _db.SubCategory.Update(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            SubCategoryandCategoryViewModel _model = new SubCategoryandCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                ErrorMessage = StatusMessage
                };
                return View(_model);
           
        }
        [ActionName("GetSubCategoryList")]
        public async Task<IActionResult> GetSubCategoryList(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories =await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }
    }
}