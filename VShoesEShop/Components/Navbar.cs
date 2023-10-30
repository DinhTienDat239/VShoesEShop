using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VShoesEShop.Data;

namespace VShoesEShop.Components
{
    public class Navbar:ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public Navbar(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.Categories.ToList());
        }
    }
}
