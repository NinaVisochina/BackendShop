using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendShop.Core.Dto.Category;
using BackendShop.Core.Interfaces;
using BackendShop.Core.Services;
using BackendShop.Data.Data;
using BackendShop.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendShop.BackShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ShopDbContext _context, IMapper mapper, IImageHulk imageHulk, IConfiguration configuration) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetList()
        {
            Thread.Sleep(1000);
            var list = _context.Categories
                .ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
                .ToList();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryCreateViewModel model)
        {
            var imageName = await imageHulk.Save(model.ImageCategory);
            var entity = mapper.Map<Category>(model);
            entity.ImageCategoryPath = imageName;
            _context.Categories.Add(entity);
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _context.Categories.SingleOrDefault(x => x.CategoryId == id);
            if (entity == null)
                return NotFound();
            if (!string.IsNullOrEmpty(entity.ImageCategoryPath))
                imageHulk.Delete(entity.ImageCategoryPath);
            _context.Categories.Remove(entity);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] CategoryEditViewModel model)
        {
            var entity = await _context.Categories.SingleOrDefaultAsync(x => x.CategoryId == model.Id);
            if (entity == null)
                return NotFound();

            // Update basic fields
            entity.Name = model.Name;

            // Handle image update
            if (model.Image != null && model.Image.Length > 0)
            {
                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(entity.ImageCategoryPath))
                {
                    imageHulk.Delete(entity.ImageCategoryPath);
                }

                // Save the new image
                var newImageName = await imageHulk.Save(model.Image);
                entity.ImageCategoryPath = newImageName;
            }

            _context.Categories.Update(entity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.Categories
                .ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
                .SingleOrDefault(x => x.Id == id);
            return Ok(item);
        }

        //[HttpGet("names")]
        //public async Task<IActionResult> GetCategoriesNames()
        //{
        //    var result = await context.Categories
        //    .ProjectTo<SelectItemViewModel>(mapper.ConfigurationProvider)
        //    .ToListAsync();
        //    return Ok(result);
        //}
    }
}
