using Avalonia.Controls;
using Avalonia.Interactivity;
using HIS.Portal.Contracts;
using HIS.Portal.Navigation;
using HIS.Portal.Modules;
using HIS.Portal.Modules.Home;
using HIS.Portal.Modules.Outpatient;
using HIS.Portal.Modules.WebDemo;
using HIS.Portal.Shell;

namespace HIS.Portal.Modules.Home;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private void OnRecentVisitClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: VisitRecord visit } && DataContext is HomeViewModel viewModel)
            viewModel.OpenVisitCommand.Execute(visit);
    }
}
