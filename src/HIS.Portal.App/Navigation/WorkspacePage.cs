using System.ComponentModel;

namespace HIS.Portal.Navigation;

public enum CloseDecision
{
    Save,
    Discard,
    Cancel,
}

/// <summary>
/// View-model contract used by the shell when a document tab is closed. It deliberately has no
/// Avalonia dependency so the save/discard/cancel policy can be tested without a UI thread.
/// </summary>
public interface IWorkspacePageViewModel : INotifyPropertyChanged
{
    string Title { get; }

    bool IsDirty { get; }

    event EventHandler? CloseRequested;

    Task<bool> SaveAsync();

    void DiscardChanges();

    void Release();
}

/// <summary>
/// A document tab owned by <see cref="TabManager"/>. The shell hosts <see cref="View"/> instances
/// long-term in its PageHost; the tab strip only owns the header.
/// </summary>
public interface IWorkspacePage
{
    PageKey Key { get; }

    string Title { get; }

    bool IsClosable { get; }

    IWorkspacePageViewModel ViewModel { get; }

    void Release();
}

public sealed class WorkspacePageEventArgs(IWorkspacePage page) : EventArgs
{
    public IWorkspacePage Page { get; } = page;
}
