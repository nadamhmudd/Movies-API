﻿using Movies.Services.Helpers.FilesHnadler;

namespace Movies.Services.Helpers;
public class ImagesHandler : BaseFileHandler
{
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
    private const long _maxAllowedSize = 1048576; //one megebyte
    private const long _megabyte = 1024*1024;

    public ImagesHandler()
    {
        allowedExtensions = _allowedExtensions;
        maxAllowedSize = _maxAllowedSize;
        megabyte = _megabyte;
    }
}
