global using Microsoft.AspNetCore.Http;
global using Movies.Core.Interfaces;

namespace Movies.Services.Helpers.FilesHnadler
{
    public static class BaseFileHandler //: IBaseFileHandler
    {
        //protected  static string[] allowedExtensions;
        //protected const long maxAllowedSize;
        //protected const long megabyte;
        private static string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
        private const long maxAllowedSize = 1048576; //one megebyte
        private const long megabyte = 1024 * 1024;


        //public BaseFileHandler()
        //{
        //    //Initialize 
        //    Image = new();
        //}
        //public ImageHandler Image { get; private set; }

        //global method
        public static async Task<string> Upload(IFormFile file, string path)
        {
            var name = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return $"Only [{string.Join(", ", allowedExtensions)}] files are allowed.";

            if (file.Length > maxAllowedSize)
                return $"Max allowed size for image is {maxAllowedSize / megabyte}MB.";

            using var fileStream = new FileStream(Path.Combine(path, name + extension), FileMode.Create);
            await file.CopyToAsync(fileStream);

            return @$"{path}\{name + extension}";
        }
        public static void Delete(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
