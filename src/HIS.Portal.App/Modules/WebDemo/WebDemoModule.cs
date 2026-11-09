using HIS.Portal.Contracts;
using HIS.Portal.Diagnostics;
using HIS.Portal.Navigation;

namespace HIS.Portal.Modules.WebDemo;

/// <summary>Owns the local-HTML WebView demo composition and its lifecycle diagnostics.</summary>
public sealed class WebDemoModule(RuntimeDiagnostics diagnostics) : IPageModule
{
    public bool CanHandle(PageKind kind) => kind == PageKind.WebDemo;

    public IWorkspacePage Create(PageKey key)
    {
        if (!CanHandle(key.Kind))
            throw new ArgumentOutOfRangeException(nameof(key), key.Kind, "WebDemoModule cannot create this page.");

        var viewModel = new WebDemoViewModel();
        var view = new WebDemoView(diagnostics) { DataContext = viewModel };
        return new PageDocument(key, viewModel.Title, view, viewModel, diagnostics);
    }
}
