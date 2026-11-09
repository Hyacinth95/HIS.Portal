using HIS.Portal.Contracts;
using HIS.Portal.Diagnostics;
using HIS.Portal.Navigation;

namespace HIS.Portal.Modules.Home;

/// <summary>Owns the Home module composition (view + view model + document lifetime).</summary>
public sealed class HomeModule(
    IVisitRepository repository,
    Action<PageKey> navigate,
    RuntimeDiagnostics diagnostics) : IPageModule
{
    public bool CanHandle(PageKind kind) => kind == PageKind.Home;

    public IWorkspacePage Create(PageKey key)
    {
        if (!CanHandle(key.Kind))
            throw new ArgumentOutOfRangeException(nameof(key), key.Kind, "HomeModule cannot create this page.");

        var viewModel = new HomeViewModel(repository, navigate);
        var view = new HomeView { DataContext = viewModel };
        return new PageDocument(key, viewModel.Title, view, viewModel, diagnostics, isClosable: false);
    }
}
