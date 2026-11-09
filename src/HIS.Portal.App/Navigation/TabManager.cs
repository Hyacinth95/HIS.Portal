using System.Collections.ObjectModel;

namespace HIS.Portal.Navigation;

/// <summary>
/// Owns the open-document list. It knows nothing about Avalonia controls, which keeps the
/// de-duplication, activation and close policy testable without a UI thread.
/// </summary>
public sealed class TabManager
{
    private readonly ObservableCollection<IWorkspacePage> _documents = [];
    private readonly HashSet<PageKey> _closing = [];
    private readonly ReadOnlyObservableCollection<IWorkspacePage> _readOnlyDocuments;
    private IWorkspacePage? _active;

    public TabManager()
    {
        _readOnlyDocuments = new ReadOnlyObservableCollection<IWorkspacePage>(_documents);
    }

    public ReadOnlyObservableCollection<IWorkspacePage> Documents => _readOnlyDocuments;

    public IWorkspacePage? Active => _active;

    /// <summary>
    /// UI shell hook used to ask the user to save, discard, or cancel a close.
    /// </summary>
    public Func<IWorkspacePage, Task<CloseDecision>>? ClosePrompt { get; set; }

    public event EventHandler? ActiveChanged;

    public event EventHandler<WorkspacePageEventArgs>? PageOpened;

    public event EventHandler<WorkspacePageEventArgs>? PageReleasing;

    public event EventHandler<WorkspacePageEventArgs>? PageClosed;

    public IWorkspacePage? Find(PageKey key) =>
        _documents.FirstOrDefault(page => page.Key == key);

    public IWorkspacePage Open(IWorkspacePage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        var existing = Find(page.Key);
        if (existing is not null)
        {
            Activate(existing);
            return existing;
        }

        _documents.Add(page);
        PageOpened?.Invoke(this, new WorkspacePageEventArgs(page));
        SetActive(page);
        return page;
    }

    public void Activate(IWorkspacePage page)
    {
        ArgumentNullException.ThrowIfNull(page);
        if (!_documents.Contains(page))
            throw new InvalidOperationException($"Page '{page.Key}' is not open.");

        SetActive(page);
    }

    public void Activate(PageKey key)
    {
        var page = Find(key) ?? throw new InvalidOperationException($"Page '{key}' is not open.");
        SetActive(page);
    }

    /// <summary>
    /// Closes a page. A dirty page asks <see cref="ClosePrompt"/> unless a decision is supplied
    /// by a deterministic caller such as an automated test.
    /// </summary>
    public async Task<bool> TryCloseAsync(IWorkspacePage page, CloseDecision? decision = null)
    {
        ArgumentNullException.ThrowIfNull(page);
        if (!page.IsClosable)
            return false;
        if (!_documents.Contains(page))
            return false;
        if (!_closing.Add(page.Key))
            return false;

        try
        {
            if (page.ViewModel.IsDirty)
            {
                var actualDecision = decision
                    ?? (ClosePrompt is null
                        ? CloseDecision.Cancel
                        : await ClosePrompt(page).ConfigureAwait(true));

                switch (actualDecision)
                {
                    case CloseDecision.Cancel:
                        return false;
                    case CloseDecision.Discard:
                        page.ViewModel.DiscardChanges();
                        break;
                    case CloseDecision.Save:
                        if (!await page.ViewModel.SaveAsync().ConfigureAwait(true))
                            return false;
                        break;
                    default:
                        return false;
                }
            }

            var wasActive = ReferenceEquals(page, _active);
            var index = _documents.IndexOf(page);
            IWorkspacePage? next = null;
            if (wasActive)
            {
                if (_documents.Count > 1)
                    next = _documents[index == _documents.Count - 1 ? index - 1 : index + 1];
            }

            PageReleasing?.Invoke(this, new WorkspacePageEventArgs(page));
            _documents.Remove(page);
            page.Release();
            PageClosed?.Invoke(this, new WorkspacePageEventArgs(page));

            if (wasActive)
                SetActive(next);
            else if (_active is null && _documents.Count > 0)
                SetActive(_documents[0]);

            return true;
        }
        finally
        {
            _closing.Remove(page.Key);
        }
    }

    private void SetActive(IWorkspacePage? page)
    {
        if (ReferenceEquals(_active, page))
            return;

        _active = page;
        ActiveChanged?.Invoke(this, EventArgs.Empty);
    }
}
