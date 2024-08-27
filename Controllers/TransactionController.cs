using GeneralStoreMVC.Data;
using GeneralStoreMVC.Models.Transaction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GeneralStoreMVC.Controllers;

public class TransactionController : Controller
{
    private readonly GeneralStoreDbContext _ctx;
    public TransactionController(GeneralStoreDbContext dbContext)
    {
        _ctx = dbContext;
    }

    public async Task<IActionResult> Create(int customerId)
    {
        TransactionCreateVM model = new()
        {
            CustomerId = customerId
        };

        var productList = await _ctx.Products
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} ({p.Price:C})",
            })
            .ToListAsync();

        ViewData["Products"] = productList;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int customerId, TransactionCreateVM model)
    {
        if (customerId != model.CustomerId)
            return RedirectToAction("Index", "Customer");

        var entity = new Transaction
        {
            ProductId = model.ProductId,
            CustomerId = model.CustomerId,
            Quantity = model.Quantity,
            DateOfTransaction = DateTime.Now
        };

        _ctx.Transactions.Add(entity);
        if (await _ctx.SaveChangesAsync() == 1)
            return RedirectToAction("Details", "Customer", new { id = customerId });

        return View(model);
    }
}