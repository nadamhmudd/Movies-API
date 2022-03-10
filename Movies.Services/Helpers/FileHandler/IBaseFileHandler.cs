namespace Movies.Services.Helpers.FilesHnadler;

public interface IBaseFileHandler
{
    //Register diffrent handlers, can add VideoHandler for trailer and so on
    public ImageHandler Image { get; }
}
