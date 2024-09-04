using Avalonia.Controls;
using ImageViewer.ViewModels;

namespace ImageViewer.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}