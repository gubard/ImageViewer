using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace ImageViewer.Services;

public class DirectorySelector
{
    private readonly DirectoryInfo directory;
    private readonly ushort deep;
    private readonly ushort count;
    private DirectoryInfo currentDirectory;
    private ushort currentCount;
    private List<FileInfo> files;

    public DirectorySelector(DirectoryInfo directory, ushort deep, ushort count)
    {
        this.deep = deep;
        this.directory = directory;
        this.count = count;
        files = new();
        currentDirectory = GetRandomDirectory();
        files.AddRange(currentDirectory.GetFiles("*", SearchOption.AllDirectories));
    }

    public FileInfo GetNextFile()
    {
        if (currentCount >= count)
        {
            currentCount = 0;
            currentDirectory = GetRandomDirectory();
            files.AddRange(currentDirectory.GetFiles("*", SearchOption.AllDirectories));

            if (files.Count == 0)
            {
                currentDirectory.Delete();
            }
        }

        if (files.Count == 0)
        {
            currentCount = count;

            return GetNextFile();
        }

        currentCount++;
        var result = files[RandomNumberGenerator.GetInt32(0, files.Count)];
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
                throw new DirectoryNotFoundException();
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
