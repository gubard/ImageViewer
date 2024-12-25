using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace ImageViewer.Services;

public class DirectorySelector
{
    public ulong FilesCount { get; private set; }
    public ulong FilesCountSaw { get; private set; }

    public DirectorySelector(DirectoryInfo rootDirectory)
    {
        this.rootDirectory = rootDirectory;
        directories = [..rootDirectory.GetDirectories("*", SearchOption.AllDirectories),];
        currentDirectory = GetNextDirectory();
        files = [..currentDirectory.GetFiles("*", SearchOption.AllDirectories),];
        FilesCount = (ulong)files.Count;
    }

    public FileInfo GetNextFile()
    {
        if (files.Count == 0)
        {
            currentDirectory = GetNextDirectory();
            files.AddRange(currentDirectory.GetFiles("*", SearchOption.AllDirectories));
            FilesCount = (ulong)files.Count;
            FilesCountSaw = 0;

            if (files.Count == 0)
            {
                throw new(currentDirectory.ToString());
            }
        }

        var result = files[0];
        FilesCountSaw++;
        files.Remove(result);

        return result;
    }

    private DirectoryInfo GetNextDirectory()
    {
        if (directories.Count == 0)
        {
            directories.AddRange(rootDirectory.GetDirectories("*", SearchOption.AllDirectories));
        }

        var result = directories[RandomNumberGenerator.GetInt32(0, directories.Count)];

        return result;
    }
    
    private readonly DirectoryInfo rootDirectory;
    private DirectoryInfo currentDirectory;
    private readonly List<DirectoryInfo> directories;
    private readonly List<FileInfo> files;
}