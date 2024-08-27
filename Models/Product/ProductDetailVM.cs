using System.ComponentModel.DataAnnotations;

namespace GeneralStoreMVC.Models.Product;

public class ProductDetailVM
{
    public int Id { get; set; }

    [Display(Name = "Product Name")]
    public string? Name { get; set; }

    [Display(Name = "Quantity In Stock")]
    public int QuantityInStock { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Price Per")]
    public double Price { get; set; }
}