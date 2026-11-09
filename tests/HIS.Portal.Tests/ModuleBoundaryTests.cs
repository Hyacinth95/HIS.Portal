using System.ComponentModel;
using HIS.Portal.Contracts;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Demo;
using HIS.Portal.Infrastructure.Nest;
using HIS.Portal.Navigation;

namespace HIS.Portal.Tests;

public class ModuleBoundaryTests
{
    [Fact]
    public void NavigationCatalog_CoversM1PrimaryPagesWithUniqueKeys()
    {
        var keys = NavigationCatalog.Features.Select(feature => feature.Key).ToArray();

        Assert.Equal(3, keys.Length);
        Assert.Contains(PageKey.Home, keys);
        Assert.Contains(PageKey.VisitList, keys);
        Assert.Contains(PageKey.WebDemo, keys);
        Assert.Equal(keys.Length, keys.Distinct().Count());
        Assert.All(keys, key => Assert.NotNull(NavigationCatalog.Find(key)));
    }

    [Fact]
    public void DemoModuleFactory_ReportsOnlyImplementedKinds()
    {
        var factory = new DemoModuleFactory(
            new DemoVisitRepository(),
            _ => { },
            new RuntimeDiagnostics());

        Assert.True(factory.CanCreate(PageKind.Home));
        Assert.True(factory.CanCreate(PageKind.VisitList));
        Assert.True(factory.CanCreate(PageKind.VisitDetail));
        Assert.True(factory.CanCreate(PageKind.WebDemo));
        Assert.False(factory.CanCreate((PageKind)999));
    }

    [Fact]
    public void PageRegistry_DelegatesCreationToModuleFactory()
    {
        var factory = new RecordingModuleFactory();
        var registry = new PageRegistry(factory);
        var key = PageKey.ForVisit("MZ20261108-001");

        var page = registry.Create(key);

        Assert.True(factory.CreateCalled);
        Assert.Equal(key, page.Key);
        Assert.True(registry.CanCreate(PageKind.Home));
    }

    [Fact]
    public void PageRegistry_RejectsKindTheFactoryDoesNotSupport()
    {
        var registry = new PageRegistry(new RecordingModuleFactory());

        Assert.Throws<ArgumentOutOfRangeException>(
            () => registry.Create(new PageKey((PageKind)999, "unsupported")));
    }

    [Fact]
    public void NestCandidate_IsDocumentedButNotConnected()
    {
        Assert.Equal("GET", NestApiCacheCandidate.HttpMethod);
        Assert.Equal("api/v2/apicache", NestApiCacheCandidate.CandidatePath);
        Assert.Equal("candidate-not-connected", NestApiCacheCandidate.Status);
        Assert.Equal(1, new NestApiCacheCandidate.Response(1).A);
    }

    private sealed class RecordingModuleFactory : IModuleFactory
    {
        public bool CreateCalled { get; private set; }

        public bool CanCreate(PageKind kind) => kind is PageKind.Home or PageKind.VisitDetail;

        public IWorkspacePage Create(PageKey key)
        {
            CreateCalled = true;
            return new FakePage(key);
        }
    }

    private sealed class FakePage(PageKey key) : IWorkspacePage
    {
        public PageKey Key { get; } = key;

        public string Title => "Fake";

        public bool IsClosable => true;

        public IWorkspacePageViewModel ViewModel { get; } = new FakeViewModel();

        public void Release()
        {
        }
    }

    private sealed class FakeViewModel : IWorkspacePageViewModel
    {
        public string Title => "Fake";

        public bool IsDirty => false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler? CloseRequested;

        public Task<bool> SaveAsync() => Task.FromResult(true);

        public void DiscardChanges()
        {
        }

        public void Release()
        {
        }
    }
}
