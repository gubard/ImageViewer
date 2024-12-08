using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ImageViewer.Services;

public class DirectorySelector
{
    private readonly DirectoryInfo directory;
    private readonly ushort deep;
    private DirectoryInfo currentDirectory;
    private List<FileInfo> files;

    public DirectorySelector(DirectoryInfo directory, ushort deep)
    {
        this.deep = deep;
        this.directory = directory;
        files = new();
        currentDirectory = GetRandomDirectory();
        files.AddRange(currentDirectory.GetFiles("*", SearchOption.AllDirectories));
    }

    public FileInfo GetNextFile()
    {
        if (files.Count == 0)
        {
            currentDirectory = GetRandomDirectory();
            files.AddRange(currentDirectory.GetFiles("*", SearchOption.AllDirectories));

            if (files.Count == 0)
            {
                currentDirectory.Delete(true);
                
               return GetNextFile();
            }
        }
        
        var result = files[0];
        files.Remove(result);

        return result;
    }

    private DirectoryInfo GetRandomDirectory()
    {
        var result = directory;

        for (var i = 0; i < deep; i++)
        {
            var directories = result.GetDirectories();

            if (directories.Length == 0)
            {
                i = 0;
                result = directory;

                continue;
            }

            if (directories.Length == 1)
            {
                result = directories[0];

                continue;
            }

            result = directories[RandomNumberGenerator.GetInt32(0, directories.Length)];
        }

        return result;
    }
}
