﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        //Get
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);

        }
        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        //GET - EDIT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var deleteItem =await _db.Category.FindAsync(id);
            if (deleteItem == null)
            {
                return NotFound();
            }
            else
            {
                _db.Category.Remove(deleteItem);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
    }
}