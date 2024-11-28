using System.ComponentModel.DataAnnotations;

namespace BackendShop.Data.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public string? Manufacturer { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }

        public string? Type { get; set; }


        public string? Form { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public int? QuantityInPack { get; set; }
        [Required]
        public int QuantityInStock { get; set; }
       // public bool IsAvailable { get; set; }

        public string? Model { get; set; }
        public int SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }
        public  ICollection<ProductImageEntity>? Images { get; set; }
    }
}
