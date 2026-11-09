using System.ComponentModel;
using HIS.Portal.Navigation;

namespace HIS.Portal.Tests;

public class TabManagerTests
{
    [Fact]
    public void SameKey_ReusesExistingDocumentAndActivatesIt()
    {
        var tabs = new TabManager();
        var first = new FakePage(new PageKey(PageKind.VisitDetail, "V001"), "第一次");
        var duplicate = new FakePage(new PageKey(PageKind.VisitDetail, "V001"), "重复");

        tabs.Open(first);
        tabs.Open(duplicate);

        Assert.Single(tabs.Documents);
        Assert.Same(first, tabs.Active);
        Assert.Same(first, tabs.Find(first.Key));
        Assert.Equal(0, duplicate.ReleaseCount);
    }

    [Fact]
    public void DifferentBusinessIds_CanStayOpenTogether()
    {
        var tabs = new TabManager();
        var first = new FakePage(new PageKey(PageKind.VisitDetail, "V001"), "第一位");
        var second = new FakePage(new PageKey(PageKind.VisitDetail, "V002"), "第二位");

        tabs.Open(first);
        tabs.Open(second);

        Assert.Equal(2, tabs.Documents.Count);
        Assert.Same(second, tabs.Active);
        Assert.NotSame(first, second);
    }

    [Fact]
    public void TenActivations_KeepSameDraftAndDirtyState()
    {
        var tabs = new TabManager();
        var first = new FakePage(new PageKey(PageKind.VisitDetail, "V001"), "第一位");
        var second = new FakePage(new PageKey(PageKind.VisitList, "list"), "列表");
        tabs.Open(first);
        tabs.Open(second);

        first.ViewModel.IsDirty = true;
        first.ViewModel.Notes = "未保存草稿";
        for (var i = 0; i < 10; i++)
            tabs.Activate(i % 2 == 0 ? first : second);

        tabs.Activate(first);

        Assert.Same(first, tabs.Active);
        Assert.True(first.ViewModel.IsDirty);
        Assert.Equal("未保存草稿", first.ViewModel.Notes);
        Assert.Equal(2, tabs.Documents.Count);
    }

    [Fact]
    public async Task CancelClose_KeepsDirtyDocumentAndDraft()
    {
        var tabs = new TabManager();
        var page = new FakePage(new PageKey(PageKind.VisitDetail, "V001"), "详情");
        tabs.Open(page);
        page.ViewModel.IsDirty = true;
        tabs.ClosePrompt = _ => Task.FromResult(CloseDecision.Cancel);

        var closed = await tabs.TryCloseAsync(page);

        Assert.False(closed);
        Assert.Single(tabs.Documents);
        Assert.True(page.ViewModel.IsDirty);
        Assert.Equal(0, page.ReleaseCount);
        Assert.Equal(0, page.ViewModel.SaveCount);
        Assert.Equal(0, page.ViewModel.DiscardCount);
    }

    [Fact]
    public async Task SaveClose_WritesAndReleasesDocument()
    {
        var tabs = new TabManager();
        var page = new FakePage(new PageKey(PageKind.VisitDetail, "V001"), "详情");
        tabs.Open(page);
        page.ViewModel.IsDirty = true;
        page.ViewModel.Notes = "草稿";

        var closed = await tabs.TryCloseAsync(page, CloseDecision.Save);

        Assert.True(closed);
        Assert.Empty(tabs.Documents);
        Assert.Equal(1, page.ViewModel.SaveCount);
        Assert.Equal(1, page.ReleaseCount);
        Assert.False(page.ViewModel.IsDirty);
    }

    [Fact]
    public async Task DiscardClose_DoesNotCallSave()
    {
        var tabs = new TabManager();
        var page = new FakePage(new PageKey(PageKind.VisitDetail, "V001"), "详情");
        tabs.Open(page);
        page.ViewModel.IsDirty = true;

        var closed = await tabs.TryCloseAsync(page, CloseDecision.Discard);

        Assert.True(closed);
        Assert.Equal(0, page.ViewModel.SaveCount);
        Assert.Equal(1, page.ViewModel.DiscardCount);
        Assert.Equal(1, page.ReleaseCount);
    }

    [Fact]
    public async Task FailedSave_DoesNotClosePage()
    {
        var tabs = new TabManager();
        var page = new FakePage(new PageKey(PageKind.VisitDetail, "V001"), "详情");
        tabs.Open(page);
        page.ViewModel.IsDirty = true;
        page.ViewModel.SaveSucceeds = false;

        var closed = await tabs.TryCloseAsync(page, CloseDecision.Save);

        Assert.False(closed);
        Assert.Single(tabs.Documents);
        Assert.Equal(1, page.ViewModel.SaveCount);
        Assert.Equal(0, page.ReleaseCount);
    }

    [Fact]
    public async Task NonClosablePage_CannotBeClosed()
    {
        var tabs = new TabManager();
        var home = new FakePage(PageKey.Home, "首页", isClosable: false);
        tabs.Open(home);

        var closed = await tabs.TryCloseAsync(home, CloseDecision.Discard);

        Assert.False(closed);
        Assert.Single(tabs.Documents);
        Assert.Equal(0, home.ReleaseCount);
    }

    private sealed class FakePage : IWorkspacePage
    {
        public FakePage(PageKey key, string title, bool isClosable = true)
        {
            Key = key;
            Title = title;
            IsClosable = isClosable;
        }

        public PageKey Key { get; }

        public string Title { get; }

        public bool IsClosable { get; }

        public FakePageViewModel ViewModel { get; } = new();

        IWorkspacePageViewModel IWorkspacePage.ViewModel => ViewModel;

        public int ReleaseCount { get; private set; }

        public void Release()
        {
            ReleaseCount++;
            ViewModel.Release();
        }
    }

    private sealed class FakePageViewModel : IWorkspacePageViewModel
    {
        private bool _isDirty;

        public string Title => "Fake";

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty == value)
                    return;
                _isDirty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
            }
        }

        public string Notes { get; set; } = string.Empty;

        public bool SaveSucceeds { get; set; } = true;

        public int SaveCount { get; private set; }

        public int DiscardCount { get; private set; }

        public int ReleaseCount { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler? CloseRequested;

        public Task<bool> SaveAsync()
        {
            SaveCount++;
            if (!SaveSucceeds)
                return Task.FromResult(false);

            IsDirty = false;
            return Task.FromResult(true);
        }

        public void DiscardChanges()
        {
            DiscardCount++;
            IsDirty = false;
        }

        public void Release() => ReleaseCount++;
    }
}
