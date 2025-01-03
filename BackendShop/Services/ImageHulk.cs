using BackendShop.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace BackendShop.Services
{
    //public class ImageHulk(IConfiguration configuration) : IImageHulk
    //{
    //    public bool Delete(string fileName)
    //    {
    //        try
    //        {
    //            var dir = configuration["ImagesDir"];
    //            var sizes = configuration["ImageSizes"].Split(",").Select(int.Parse);

    //            foreach (var size in sizes)
    //            {
    //                string path = Path.Combine(Directory.GetCurrentDirectory(), dir, $"{size}_{fileName}");
    //                if (File.Exists(path))
    //                    File.Delete(path);
    //            }
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    public async Task<string> Save(IFormFile image)
    //    {
    //        string imageName = Guid.NewGuid().ToString() + ".webp"; // Генеруємо ім'я файлу
    //        var dir = configuration["ImagesDir"];
    //        var sizes = configuration["ImageSizes"].Split(",").Select(int.Parse);

    //        using var ms = new MemoryStream();
    //        await image.CopyToAsync(ms);
    //        var bytes = ms.ToArray();

    //        foreach (var size in sizes)
    //        {
    //            string path = Path.Combine(Directory.GetCurrentDirectory(), dir, $"{size}_{imageName}");
    //            using var img = SixLabors.ImageSharp.Image.Load(bytes);
    //            img.Mutate(x => x.Resize(new ResizeOptions
    //            {
    //                Size = new Size(size, size),
    //                Mode = ResizeMode.Max
    //            }));
    //            img.Save(path, new WebpEncoder());
    //        }

    //        return imageName;
    //    }

    //    public async Task<string> Save(string urlImage)
    //    {
    //        string imageName = Guid.NewGuid().ToString() + ".webp";
    //        try
    //        {
    //            using var client = new HttpClient();
    //            var response = await client.GetAsync(urlImage);
    //            if (response.IsSuccessStatusCode)
    //            {
    //                var bytes = await response.Content.ReadAsByteArrayAsync();
    //                return SaveByteArray(bytes, imageName);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Помилка завантаження зображення: {ex.Message}");
    //        }

    //        return imageName;
    //    }

    //    private string SaveByteArray(byte[] bytes, string imageName)
    //    {
    //        var dir = configuration["ImagesDir"];
    //        var sizes = configuration["ImageSizes"].Split(",").Select(int.Parse);

    //        foreach (var size in sizes)
    //        {
    //            string path = Path.Combine(Directory.GetCurrentDirectory(), dir, $"{size}_{imageName}");
    //            using var img = SixLabors.ImageSharp.Image.Load(bytes);
    //            img.Mutate(x => x.Resize(new ResizeOptions
    //            {
    //                Size = new Size(size, size),
    //                Mode = ResizeMode.Max
    //            }));
    //            img.Save(path, new WebpEncoder());
    //        }

    //        return imageName;
    //    }
    //}

    public class ImageHulk(IConfiguration configuration) : IImageHulk
    {
        public bool Delete(string fileName)
        {
            try
            {
                var dir = configuration["ImagesDir"];
                var sizes = configuration["ImageSizes"].Split(",")
                    .Select(x => int.Parse(x));
                //int[] sizes = [50, 150, 300, 600, 1200];
                foreach (var size in sizes)
                {
                    string dirSave = Path.Combine(Directory.GetCurrentDirectory(),
                        dir, $"{size}_{fileName}");

                    if (File.Exists(dirSave))
                        File.Delete(dirSave);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> Save(IFormFile image)
        {
            string imageName = string.Empty;

            using (MemoryStream ms = new())
            {
                await image.CopyToAsync(ms);
                var bytes = ms.ToArray();
                imageName = SaveByteArray(bytes);
            }

            return imageName;
        }

        private string SaveByteArray(byte[] bytes)
        {
            string imageName = Guid.NewGuid().ToString() + ".webp";
            var dir = configuration["ImagesDir"];

            var sizes = configuration["ImageSizes"].Split(",")
                    .Select(x => int.Parse(x));
            //int[] sizes = [50, 150, 300, 600, 1200];
            foreach (var size in sizes)
            {
                string dirSave = Path.Combine(Directory.GetCurrentDirectory(),
                    dir, $"{size}_{imageName}");
                using (var imageLoad = Image.Load(bytes))
                {
                    // Resize the image (50% of original dimensions)
                    imageLoad.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(size, size),
                        Mode = ResizeMode.Max
                    }));

                    // Save the image with compression
                    imageLoad.Save(dirSave, new WebpEncoder());
                }
            }
            return imageName;
        }

        public async Task<string> Save(string urlImage)
        {
            string imageName = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Send a GET request to the image URL
                    HttpResponseMessage response = client.GetAsync(urlImage).Result;

                    // Check if the response status code indicates success (e.g., 200 OK)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the image bytes from the response content
                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                        imageName = SaveByteArray(imageBytes);
                    }
                }
            }
            catch
            {
                return imageName;
            }
            return imageName;
        }
    }
}
