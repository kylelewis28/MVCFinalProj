using Microsoft.AspNetCore.Mvc;
using GeneralStoreMVC.Data;
using GeneralStoreMVC.Models.Customer;
using Microsoft.EntityFrameworkCore;
using GeneralStoreMVC.Models.Transaction;
//We're injecting our GeneralStoreDbContext so we can use it throughout our various controller methods.
namespace GeneralStoreMVC.Controllers;

public class CustomerController : Controller
{
    private readonly GeneralStoreDbContext _ctx;
    public CustomerController(GeneralStoreDbContext dbContext)
    {
        _ctx = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        List<CustomerIndexViewModel> customers = await _ctx.Customers
            .Select(customer => new CustomerIndexViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email
            })
            .ToListAsync();

        return View(customers);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMsg"] = "Model State is invalid.";
            return View(model);
        }

        Customer entity = new()
        {
            Name = model.Name,
            Email = model.Email
        };
        _ctx.Customers.Add(entity);

        if (await _ctx.SaveChangesAsync() != 1)
        {
            TempData["ErrorMsg"] = "Unable to save to the database. Please try again later.";
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }
    // GET: customer/edit/{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var entity = await _ctx.Customers.FindAsync(id);
        if (entity is null)
        {
            return RedirectToAction(nameof(Index));
        }

        CustomerEditViewModel model = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email
        };
        return View(model);
    }




    
    // GET: customer/details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var entity = await _ctx.Customers
        .Include(c => c.Transactions)
        .ThenInclude(t => t.Product)
        .FirstOrDefaultAsync(c => c.Id == id);


        if (entity is null)
        {
            return RedirectToAction(nameof(Index));
        }
        var transactions = entity.Transactions
        .Select(t => new TransactionListItem
        {
            ProductName = t.Product.Name,
            Quantity = t.Quantity,
            DateOfTransaction = t.DateOfTransaction,
            Price = t.Product.Price * t.Quantity
        }).ToList();

        CustomerDetailViewModel model = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Transactions = transactions

        };
        return View(model);
    }


    // POST: customer/edit/{id}
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, CustomerEditViewModel model)
    {
        var entity = _ctx.Customers.Find(id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.Name = model.Name;
        entity.Email = model.Email;
        _ctx.Entry(entity).State = EntityState.Modified;

        if (_ctx.SaveChanges() == 1)
        {
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMsg"] = "Unable to save to the database. Please try again later.";
        return View(model);
    }
    // GET: customer/delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _ctx.Customers
            .Include(c => c.Transactions)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (entity is null)
        {
            TempData["ErrorMsg"] = $"Customer #{id} does not exist";
            return RedirectToAction(nameof(Index));
        }

        if (entity.Transactions.Count > 0)
        {
            _ctx.Transactions.RemoveRange(entity.Transactions);
        }

        _ctx.Customers.Remove(entity);

        if (_ctx.SaveChanges() != 1 + entity.Transactions.Count)
        {
            TempData["ErrorMsg"] = $"Cannot delete Customer #{id}";
        }

        return RedirectToAction(nameof(Index));
    }
}