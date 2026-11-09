using Avalonia.Controls;
using HIS.Portal.Contracts;
using HIS.Portal.Navigation;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Demo;

namespace HIS.Portal.Navigation;

/// <summary>
/// Pairs a page view model with the long-lived Avalonia view that lives in the page host.
/// The TabView only owns the tab header, so the view is never rebuilt by a tab switch.
/// </summary>
public sealed class PageDocument : IWorkspacePage, IDisposable
{
    private readonly RuntimeDiagnostics _diagnostics;
    private bool _released;

    public PageDocument(
        PageKey key,
        string title,
        Control view,
        IWorkspacePageViewModel viewModel,
        RuntimeDiagnostics diagnostics,
        bool isClosable = true)
    {
        Key = key;
        Title = title;
        View = view;
        ViewModel = viewModel;
        _diagnostics = diagnostics;
        IsClosable = isClosable;
        _diagnostics.RecordPageCreated(key.Value);
    }

    public PageKey Key { get; }

    public string Title { get; }

    public Control View { get; }

    public bool IsClosable { get; }

    public IWorkspacePageViewModel ViewModel { get; }

    public string TabGlyph => Key.Kind switch
    {
        PageKind.Home => "\uE80F",
        PageKind.VisitList => "\uE8FD",
        PageKind.VisitDetail => "\uE77B",
        PageKind.WebDemo => "\uE774",
        _ => "\uE8A5",
    };

    public bool IsReleased => _released;

    public void Release()
    {
        if (_released)
            return;

        _released = true;
        (View as IDisposable)?.Dispose();
        ViewModel.Release();
        _diagnostics.RecordPageReleased(Key.Value);
    }

    public void Dispose() => Release();
}
