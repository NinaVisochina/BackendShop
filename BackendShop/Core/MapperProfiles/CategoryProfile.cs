using AutoMapper;
using BackendShop.Core.Dto;
using BackendShop.Data.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackendShop.Core.MapperProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(x => x.ImageCategory, opt => opt.MapFrom(x =>
                    string.IsNullOrEmpty(x.ImageCategoryPath) ? "/images/noimage.jpg" : $"/images/{x.ImageCategoryPath}"));
        }
    }
}
