namespace Movies.Services.Helpers.FilesHnadler;
public class ImageHandler : BaseHandler
{
    private static string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
    private const long _maxAllowedSize = 1048576; //one megebyte
    private const long _megabyte = 1024 * 1024;
    public ImageHandler()
    {
        allowedExtensions = _allowedExtensions;
        maxAllowedSize = _maxAllowedSize;
        megabyte = _megabyte;
    }
}
