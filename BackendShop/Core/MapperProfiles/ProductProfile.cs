using AutoMapper;
using BackendShop.Core.Dto;
using BackendShop.Data.Entities;

namespace BackendShop.Core.MapperProfiles
{
    public class ProductProfile: Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images != null ? src.Images.Select(img => $"/images/{img.Image}").ToList() : new List<string>()))
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory != null ? src.SubCategory.Name : string.Empty));
        }
    }
}
