using HIS.Portal.Contracts;
using HIS.Portal.Diagnostics;
using HIS.Portal.Navigation;

namespace HIS.Portal.Modules.Outpatient;

/// <summary>Owns the outpatient list/detail module composition.</summary>
public sealed class OutpatientModule(
    IVisitRepository repository,
    Action<PageKey> navigate,
    RuntimeDiagnostics diagnostics) : IPageModule
{
    public bool CanHandle(PageKind kind) => kind is PageKind.VisitList or PageKind.VisitDetail;

    public IWorkspacePage Create(PageKey key)
    {
        if (!CanHandle(key.Kind))
            throw new ArgumentOutOfRangeException(nameof(key), key.Kind, "OutpatientModule cannot create this page.");

        return key.Kind switch
        {
            PageKind.VisitList => CreateList(key),
            PageKind.VisitDetail => CreateDetail(key),
            _ => throw new ArgumentOutOfRangeException(nameof(key), key.Kind, "Unsupported outpatient page."),
        };
    }

    private IWorkspacePage CreateList(PageKey key)
    {
        var viewModel = new VisitListViewModel(repository, navigate);
        var view = new VisitListView { DataContext = viewModel };
        return new PageDocument(key, viewModel.Title, view, viewModel, diagnostics);
    }

    private IWorkspacePage CreateDetail(PageKey key)
    {
        var record = repository.Find(key.BusinessId)
                     ?? throw new InvalidOperationException($"未找到就诊号 {key.BusinessId} 的合成记录。");
        var viewModel = new VisitDetailViewModel(record, repository);
        var view = new VisitDetailView { DataContext = viewModel };
        return new PageDocument(key, viewModel.Title, view, viewModel, diagnostics);
    }
}
