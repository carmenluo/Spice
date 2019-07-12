using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CouponController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
       public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using(var ms1=new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                            coupon.Picture = p1;
                        }
                    }
                }
                else
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var DefaultFile = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                    byte[] p1 = null;
                    p1 = System.IO.File.ReadAllBytes(DefaultFile);
                    coupon.Picture = p1;
                }
                _db.Coupon.Update(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(coupon);
            
        }
        //Edit - Get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.FindAsync(id);
            if (coupon.Picture == null)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var DefaultFile = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                byte[] p1 = null;
                p1 = System.IO.File.ReadAllBytes(DefaultFile);
                coupon.Picture = p1;
            }
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }
        //Edit - Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1= files[0].OpenReadStream())
                    {
                        using (var ms1= new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                            coupon.Picture = p1;
                        }
                    }
                }
                _db.Coupon.Update(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(coupon);
        }
        //Edit - Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.FindAsync(id);
            if (coupon.Picture == null)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var DefaultFile = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                byte[] p1 = null;
                p1 = System.IO.File.ReadAllBytes(DefaultFile);
                coupon.Picture = p1;
            }
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int ? id)
        {
            var coupon = await _db.Coupon.FindAsync(id);
            _db.Coupon.Remove(coupon);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}