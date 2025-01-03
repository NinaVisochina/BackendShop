using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendShop.Core.Dto.Product;
using BackendShop.Core.Interfaces;
using BackendShop.Services;
using BackendShop.Data.Data;
using BackendShop.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace BackendShop.BackShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(ShopDbContext _context, IMapper mapper, IImageHulk imageHulk, IConfiguration configuration) : ControllerBase
    {
        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var model = await _context.Products
                .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
                .ToListAsync();
            return Ok(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductDto modell)
        {
            var entity = mapper.Map<Product>(modell);
            _context.Products.Add(entity);
            _context.SaveChanges();

            if (modell.ImagesDescIds.Any())
            {
                await _context.ProductDescImages
                    .Where(x => modell.ImagesDescIds.Contains(x.Id))
                    .ForEachAsync(x => x.ProductId = entity.ProductId);
            }

            if (modell.Images != null)
            {
                var p = 1;
                foreach (var image in modell.Images)
                {
                    var pi = new ProductImageEntity
                    {
                        Image = await imageHulk.Save(image),
                        Priority = p,
                        ProductId = entity.ProductId
                    };
                    p++;
                    _context.ProductImageEntity.Add(pi);
                    await _context.SaveChangesAsync();
                }
            }
            return Created();
        }

        ////Post: api/Product
        //[HttpPost]
        //public async Task<IActionResult> Create([FromForm] CreateProductDto modell)
        //{
        //    var entity = mapper.Map<Product>(modell);
        //    _context.Products.Add(entity);
        //    _context.SaveChanges();

        //    if (modell.ImagesDescIds.Any())
        //    {
        //        await _context.ProductDescImages
        //            .Where(x => modell.ImagesDescIds.Contains(x.Id))
        //            .ForEachAsync(x => x.ProductId = entity.ProductId);
        //    }

        //    if (modell.Images != null)
        //    {
        //        var p = 1;
        //        foreach (var image in modell.Images)
        //        {
        //            var pi = new ProductImageEntity
        //            {
        //                Image = await imageHulk.Save(image),
        //                Priority = p,
        //                ProductId = entity.ProductId
        //            };
        //            p++;
        //            _context.ProductImageEntity.Add(pi);
        //            await _context.SaveChangesAsync();
        //        }
        //    }
        //    return Created();
        //}
        //[HttpPost]
        //public async Task<IActionResult> Create([FromForm] CreateProductDto modell)
        //{
        //    try
        //    {
        //        // Мапимо DTO на сутність
        //        var entity = mapper.Map<Product>(modell);
        //        _context.Products.Add(entity);
        //        await _context.SaveChangesAsync();

        //        // Додаткові зображення опису
        //        if (modell.ImagesDescIds.Any())
        //        {
        //            await _context.ProductDescImages
        //                .Where(x => modell.ImagesDescIds.Contains(x.Id))
        //                .ForEachAsync(x => x.ProductId = entity.ProductId);
        //        }
        //        if (modell.Images != null && modell.Images.Any())
        //            {
        //                foreach (var image in modell.Images)
        //                {
        //                    var savedPath = await imageHulk.Save(image);
        //                    Console.WriteLine($"Файл збережено: {savedPath}");
        //                }
        //            }

        //            return CreatedAtAction(nameof(GetProductById), new { id = 1 }, modell);


        //        //// Збереження зображень продукту
        //        //if (modell.Images != null && modell.Images.Any())
        //        //{
        //        //    var priority = 1;
        //        //    foreach (var image in modell.Images)
        //        //    {
        //        //        // Зберігаємо файл і отримуємо шлях
        //        //        var savedImagePath = await imageHulk.Save(image);

        //        //        // Додаємо інформацію про зображення до бази даних
        //        //        var productImage = new ProductImageEntity
        //        //        {
        //        //            Image = savedImagePath,
        //        //            Priority = priority,
        //        //            ProductId = entity.ProductId
        //        //        };

        //        //        priority++;
        //        //        _context.ProductImageEntity.Add(productImage);
        //        //    }

        //        //    await _context.SaveChangesAsync();
        //        //}

        //        ////Повертаємо результат
        //        //return CreatedAtAction(nameof(GetProductById), new { id = entity.ProductId }, new
        //        //{
        //        //    productId = entity.ProductId,
        //        //    message = "Продукт успішно створено"
        //        //});

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Помилка при створенні продукту: {ex.Message}");
        //        return BadRequest(new { error = "Помилка створення продукту" });
        //    }
        //}

        // GET: api/Product/2
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return Ok(product);
        }
        // PUT: api/Product/2
        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] EditProductDto model)
        {
            var request = this.Request;
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == model.Id);

            mapper.Map(model, product);

            var oldNameImages = model.Images.Where(x => x.ContentType.Contains("old-image"))
                .Select(x => x.FileName) ?? [];

            var imgToDelete = product?.Images?.Where(x => !oldNameImages.Contains(x.Image)) ?? [];
            foreach (var imgDel in imgToDelete)
            {
                _context.ProductImageEntity.Remove(imgDel);
                imageHulk.Delete(imgDel.Image);
            }

            if (model.Images is not null)
            {
                int index = 0;
                foreach (var image in model.Images)
                {
                    if (image.ContentType == "old-image")
                    {
                        var oldImage = product?.Images?.FirstOrDefault(x => x.Image == image.FileName)!;
                        oldImage.Priority = index;
                    }
                    else
                    {
                        var imagePath = await imageHulk.Save(image);
                        _context.ProductImageEntity.Add(new ProductImageEntity
                        {
                            Image = imagePath,
                            Product = product,
                            Priority = index
                        });
                    }
                    index++;
                }
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDescImage([FromForm] ProductDescImageUploadDto model)
        {
            if (model.Image != null)
            {
                var pdi = new ProductDescImageEntity
                {
                    Image = await imageHulk.Save(model.Image),

                };
                _context.ProductDescImages.Add(pdi);
                await _context.SaveChangesAsync();
                return Ok(mapper.Map<ProductDescImageIdDto>(pdi));
            }
            return BadRequest();
        }

        [HttpGet("subcategory/{subCategoryId}")]
        public async Task<IActionResult> GetProductsBySubCategory(int subCategoryId)
        {
            var products = await _context.Products
                .Where(p => p.SubCategoryId == subCategoryId)
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Description,
                    p.Price,
                    Images = p.Images.Select(img => img.Image).ToList(),
                    DescImages = p.ProductDescImages.Select(descImg => descImg.Image).ToList()
                })
                .ToListAsync();

            if (!products.Any())
                return NotFound("Продукти для цієї підкатегорії не знайдено.");

            return Ok(products);
        }


        //DELETE: api/Product/2
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products
                .Include(x => x.Images)
                .Include(x => x.ProductDescImages)
                .SingleOrDefault(x => x.ProductId == id);
            if (product == null) return NotFound();

            if (product.Images != null)
                foreach (var p in product.Images)
                    imageHulk.Delete(p.Image);

            if (product.ProductDescImages != null)
                foreach (var p in product.ProductDescImages)
                    imageHulk.Delete(p.Image);

            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok();
        }

    }
}
