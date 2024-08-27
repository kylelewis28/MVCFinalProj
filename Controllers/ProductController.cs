using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeneralStoreMVC.Data;
using GeneralStoreMVC.Models.Product;

namespace GeneralStoreMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly GeneralStoreDbContext _context;

        public ProductController(GeneralStoreDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Select(p => new ProductIndexVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    QuantityInStock = p.QuantityInStock
                })
                .ToListAsync();

            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var vm = new ProductDetailVM
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock
            };

            return View(vm);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,QuantityInStock,Price")] ProductCreateVM product)
        {
            if (ModelState.IsValid)
            {
                var entity = new Product
                {
                    Name = product.Name,
                    Price = product.Price,
                    QuantityInStock = product.QuantityInStock
                };

                _context.Products.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var vm = new ProductEditVM
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock
            };
            return View(vm);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,QuantityInStock,Price")] ProductEditVM product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var entity = await _context.Products.FindAsync(id);
                if (entity is null)
                    return RedirectToAction(nameof(Index));

                entity.Name = product.Name;
                entity.Price = product.Price;
                entity.QuantityInStock = product.QuantityInStock;

                _context.Products.Update(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Products
                .Include(c => c.Transactions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity is null)
            {
                TempData["ErrorMsg"] = $"Product #{id} does not exist";
                return RedirectToAction(nameof(Index));
            }

            if (entity.Transactions.Count > 0)
            {
                _context.Transactions.RemoveRange(entity.Transactions);
            }

            _context.Products.Remove(entity);

            if (_context.SaveChanges() != 1 + entity.Transactions.Count)
            {
                TempData["ErrorMsg"] = $"Cannot delete Product #{id}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}