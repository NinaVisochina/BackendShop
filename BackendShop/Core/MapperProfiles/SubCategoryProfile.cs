using AutoMapper;
using BackendShop.Core.Dto;
using BackendShop.Data.Entities;

namespace BackendShop.Core.MapperProfiles
{
    public class SubCategoryProfile: Profile
    {
        public SubCategoryProfile()
        {
            CreateMap<SubCategory, SubCategoryDto>()
                .ForMember(x => x.ImageSubCategory, opt => opt.MapFrom(x =>
                    string.IsNullOrEmpty(x.ImageSubCategoryPath) ? "/images/noimage.jpg" : $"/images/{x.ImageSubCategoryPath}"));
        }
    }
}
