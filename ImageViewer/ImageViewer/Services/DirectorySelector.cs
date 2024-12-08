using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace ImageViewer.Services;

public class DirectorySelector
{
    private readonly DirectoryInfo rootDirectory;
    private DirectoryInfo currentDirectory;
    private readonly List<DirectoryInfo> directories;
    private readonly List<FileInfo> files;

    public DirectorySelector(DirectoryInfo rootDirectory)
    {
        this.rootDirectory = rootDirectory;
        directories = [..rootDirectory.GetDirectories("*", SearchOption.AllDirectories),];
        currentDirectory = GetNextDirectory();
        files = [..currentDirectory.GetFiles("*", SearchOption.AllDirectories),];
    }

    public FileInfo GetNextFile()
    {
        if (files.Count == 0)
        {
            currentDirectory = GetNextDirectory();
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

    private DirectoryInfo GetNextDirectory()
    {
        if (directories.Count == 0)
        {
            directories.AddRange(rootDirectory.GetDirectories("*", SearchOption.AllDirectories));
        }

        var result = directories[RandomNumberGenerator.GetInt32(0, directories.Count)];

        return result;
    }
}