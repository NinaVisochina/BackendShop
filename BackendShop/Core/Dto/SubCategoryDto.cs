using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackendShop.Core.Dto
{
    public class SubCategoryDto
    {
        public int SubCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        //public string? Description { get; set; } = string.Empty;
        public string ImageSubCategory { get; set; } = string.Empty;
        //public int CategoryId { get; set; }
        //public virtual CategoryDto Category { get; set; }
    }
}
