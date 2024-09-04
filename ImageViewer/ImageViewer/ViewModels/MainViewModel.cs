using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ImageViewer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public const string DirectoryPathStoragePath = "./storage/DirectoryPath.txt";

    [ObservableProperty]
    private string directoryPath = string.Empty;

    [ObservableProperty]
    private IImage? currentImage;

    public MainViewModel()
    {
        if (File.Exists(DirectoryPathStoragePath))
        {
            directoryPath = File.ReadAllText(DirectoryPathStoragePath);
        }
    }

    [RelayCommand]
    private async Task RunSlideshowAsync()
    {
        if (string.IsNullOrWhiteSpace(DirectoryPath))
        {
            return;
        }

        if (!Directory.Exists(DirectoryPath))
        {
            return;
        }

        var files = Directory.GetFiles(DirectoryPath, "*", SearchOption.AllDirectories);

        if (files.Length == 0)
        {
            return;
        }

        while (true)
        {
            CurrentImage = new Bitmap(files[RandomNumberGenerator.GetInt32(0, files.Length)]);
            await Task.Delay(TimeSpan.FromSeconds(15));
        }
    }

    [RelayCommand]
    private async Task OpenDirectoryAsync()
    {
        if (
            Application.Current?.ApplicationLifetime
            is not IClassicDesktopStyleApplicationLifetime appLifetime
        )
        {
            return;
        }

        if (appLifetime.MainWindow is null)
        {
            return;
        }

        var list = await appLifetime.MainWindow.StorageProvider.OpenFolderPickerAsync(new());

        if (list.Count == 0)
        {
            return;
        }

        DirectoryPath = list[0].Path.AbsolutePath;

        if (!Directory.Exists(Path.GetDirectoryName(DirectoryPathStoragePath)))
        {
            Directory.CreateDirectory(
                Path.GetDirectoryName(DirectoryPathStoragePath) ?? string.Empty
            );
        }

        await File.WriteAllTextAsync(DirectoryPathStoragePath, DirectoryPath);
    }
}
