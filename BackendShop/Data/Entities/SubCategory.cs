using System.ComponentModel.DataAnnotations;

namespace BackendShop.Data.Entities
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryId { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;
        [StringLength(255)]
        public string? Description { get; set; } = string.Empty;
        public string? ImageSubCategoryPath { get; set; } = string.Empty;
        public List<Product>? Products { get; set; }
        public int CategoryId { get; set; }
        public  Category? Category { get; set; }
    }
}
