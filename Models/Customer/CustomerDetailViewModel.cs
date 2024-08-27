using GeneralStoreMVC.Models.Transaction;

namespace GeneralStoreMVC.Models.Customer;

public class CustomerDetailViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

     public List<TransactionListItem> Transactions { get; set; } = new();
}