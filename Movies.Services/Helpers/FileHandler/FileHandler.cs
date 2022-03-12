global using Microsoft.AspNetCore.Http;
using Movies.Core.Interfaces;

namespace Movies.Services.Helpers.FilesHnadler
{
    public class FileHandler : IFileHandler
    {

        public FileHandler()
        {
            //Initialize 
            Image = new ImageHandler();
        }
        public IBaseHandler Image { get; private set; }
    }
}
