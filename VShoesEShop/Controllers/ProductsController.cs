using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VShoesEShop.Data;
using VShoesEShop.Models;
using VShoesEShop.Models.ViewModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace VShoesEShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public int PageSize = 9;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        //[Authorize(Roles ="Admin")]
        
        public async Task<IActionResult> Index(int productPage=1)
        {
            return View(
                new ProductListViewModel
                {
                    Products = _context.Products
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                    PagingInfo = new PagingInfo
                    {
                        ItemsPerPage = PageSize,
                        CurrentPage = productPage,
                        TotalItems = _context.Products.Count()
                    }
                }

                );
        }
        public IActionResult GetfilteredProducts([FromBody] FilterData filter)
        {
            var filteredProducts = _context.Products.ToList();
            if (filter.PriceRanges != null && filter.PriceRanges.Count > 0 && !filter.PriceRanges.Contains("all"))
            {
                List<PriceRange> priceRanges = new List<PriceRange>();
                foreach (var Range in filter.PriceRanges)
                {
                    var value = Range.Split("-").ToArray();
                    PriceRange pricerange = new PriceRange();
                    pricerange.min = Int16.Parse(value[0]);
                    pricerange.max = Int16.Parse(value[1]);
                    priceRanges.Add(pricerange);
                }
                filteredProducts = filteredProducts.Where(p => priceRanges.Any(r => p.ProductPrice >= r.min && p.ProductPrice <= r.max)).ToList();

            }
            if (filter.Colors != null && filter.Colors.Count > 0 && !filter.Colors.Contains("all"))
            {

                filteredProducts = filteredProducts.Where(p => filter.Colors.Contains(p.Color.ColorName)).ToList();

            }
            if (filter.Sizes != null && filter.Sizes.Count > 0)
            {

                filteredProducts = filteredProducts.Where(p => filter.Sizes.Contains(p.Size.SizeNumber)).ToList();

            }
            return PartialView("_ReturnProduct", filteredProducts);

        }
        public async Task<IActionResult> Search(string keywords, int productPage = 1)
        {


            return View("Index",
                  new ProductListViewModel
                  {
                      Products = _context.Products
                        .Where(p => p.ProductName.Contains(keywords))
                        .Skip((productPage - 1) * PageSize)
                        .Take(PageSize),
                      PagingInfo = new PagingInfo
                      {
                          ItemsPerPage = PageSize,
     
                        CurrentPage = productPage,
     
                        TotalItems = _context.Products.Count()
                      }
                  }
     );
}
        public async Task<IActionResult> ProductsByCat(int categoryId, int productPage = 1)
        {
            var applicationDbContext = _context.Products.Where(p=>p.CategoryId == categoryId).Include(p => p.Category).Include(p => p.Color).Include(p => p.Size);
            return View("Index", new ProductListViewModel
            {
                Products = applicationDbContext,
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,

                    CurrentPage = productPage,

                    TotalItems = _context.Products.Count()
                }
            });
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Color)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorId");
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductDescription,CategoryId,ProductPrice,ProductDiscount,ProductPhoto,SizeId,ColorId,IsTrandy,IsArrived")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorId", product.ColorId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeId", product.SizeId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorId", product.ColorId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeId", product.SizeId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,CategoryId,ProductPrice,ProductDiscount,ProductPhoto,SizeId,ColorId,IsTrandy,IsArrived")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorId", product.ColorId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeId", product.SizeId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Color)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
