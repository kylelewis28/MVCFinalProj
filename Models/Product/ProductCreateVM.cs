using System.ComponentModel.DataAnnotations;

namespace GeneralStoreMVC.Models.Product;

public class ProductCreateVM
{
    [Required, MaxLength(100)]
    [Display(Name = "Product Name", Prompt = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required, Range(0, int.MaxValue)]
    [Display(Name = "Quantity In Stock", Prompt = "100")]
    public int QuantityInStock { get; set; }

    [Required, Range(0, double.MaxValue)]
    [Display(Name = "Price Per", Prompt = "11.50")]
    public double Price { get; set; }
}