using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
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
using ImageViewer.Helpers;
using ImageViewer.Services;

namespace ImageViewer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public const string DirectoryPathStoragePath = "./storage/DirectoryPath.txt";

    [ObservableProperty]
    private string directoryPath = string.Empty;

    [ObservableProperty]
    private string currentImagePath = string.Empty;

    [ObservableProperty]
    private ushort timeoutSeconds = 12;

    [ObservableProperty]
    private IImage? currentImage;

    private FileInfo? currentImageFile;

    private CancellationTokenSource cancellationTokenSource = new();

    public MainViewModel()
    {
        if (File.Exists(DirectoryPathStoragePath))
        {
            directoryPath = File.ReadAllText(DirectoryPathStoragePath);
        }
    }

    [RelayCommand]
    private void DeleteImage()
    {
        if (currentImageFile is null)
        {
            return;
        }

        if (!currentImageFile.Exists)
        {
            return;
        }

        currentImageFile.Delete();
    }

    [RelayCommand]
    private void StopSlideshow()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
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

        var directorySelector = new DirectorySelector(new(DirectoryPath), 2, 5);
        var token = cancellationTokenSource.Token;

        while (!token.IsCancellationRequested)
        {
            currentImageFile = directorySelector.GetNextFile();
            CurrentImage = new Bitmap(currentImageFile.FullName);
            CurrentImagePath = currentImageFile.FullName;

            await Wrap.IgnoreCancelAsync(
                () => Task.Delay(TimeSpan.FromSeconds(TimeoutSeconds), token)
            );
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
