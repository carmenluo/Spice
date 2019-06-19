using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModel;

namespace Spice.Areas.Admin.Controllers
{[Area("Admin")]
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        private MenuAndCategoryViewModel menuAndCategoryViewModel { get; set; }

        public MenuAndCategoryViewModel MenuAndCategoryViewModel { get => menuAndCategoryViewModel; set => menuAndCategoryViewModel = value; }

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
            return View();
        }
    }
}