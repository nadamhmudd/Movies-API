global using Microsoft.AspNetCore.Http;

namespace Movies.Services.Helpers;
public static class ImagesHelper
{
    private static string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
    private const long _maxAllowedSize = 1048576; //one megebyte
    private const long _megabyte = 1024*1024;
    public static string UploadImage(IFormFile image, string path)
    {
        var name  = Guid.NewGuid().ToString();
        var extension = Path.GetExtension(image.FileName).ToLower();

        if (!_allowedExtensions.Contains(extension))
            return $"Only [{string.Join(", ", _allowedExtensions)}] files are allowed.";

        if(image.Length > _maxAllowedSize)
            return $"Max allowed size for image is {_maxAllowedSize/_megabyte}MB.";

        using var fileStream = new FileStream(Path.Combine(path, name + extension), FileMode.Create);
        image.CopyToAsync(fileStream);

        return @$"{path}\{name+extension}";
    }

    public static void DeleteImage(string path)
    {
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path); 
    }
}
