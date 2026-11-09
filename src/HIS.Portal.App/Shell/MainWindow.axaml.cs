using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Controls.Primitives;
using HIS.Portal.Contracts;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Demo;
using HIS.Portal.Modules.Outpatient;
using HIS.Portal.Modules.WebDemo;
using HIS.Portal.Navigation;

namespace HIS.Portal.Shell;

public partial class MainWindow : Window
{
    private readonly string? _selfTestOutput;
    private readonly IVisitRepository _repository = new DemoVisitRepository();
    private readonly RuntimeDiagnostics _diagnostics = new();
    private readonly TabManager _tabs = new();
    private readonly MainWindowViewModel _viewModel;
    private readonly PageRegistry _registry;
    private readonly ObservableCollection<PageDocument> _openPages = [];
    private readonly DispatcherTimer _clockTimer;
    private readonly DispatcherTimer _toastTimer;
    private readonly HashSet<PageKey> _closingPages = [];
    private TaskCompletionSource<CloseDecision>? _closeDialogCompletion;
    private bool _syncingTabs;
    private bool _navCollapsed;
    private bool _selfTestStarted;
    private bool _tabDragSafetyVerified;

    public MainWindow()
        : this(null)
    {
    }

    public MainWindow(string? selfTestOutput)
    {
        _selfTestOutput = selfTestOutput;
        InitializeComponent();

        _viewModel = new MainWindowViewModel(_repository);
        DataContext = _viewModel;
        _registry = new PageRegistry(new DemoModuleFactory(_repository, OpenPage, _diagnostics));
        _viewModel.SetNavigator(OpenPage);

        _tabs.ClosePrompt = PromptCloseAsync;
        _tabs.PageOpened += OnTabsPageOpened;
        _tabs.PageReleasing += OnTabsPageReleasing;
        _tabs.PageClosed += OnTabsPageClosed;
        _tabs.ActiveChanged += OnTabsActiveChanged;
        _diagnostics.Changed += OnDiagnosticsChanged;
        _repository.Changed += OnRepositoryChanged;

        BusinessTabs.SelectionChanged += OnTabSelectionChanged;
        BusinessTabs.TabCloseRequested += OnTabCloseRequested;
        BusinessTabs.TabDragStarting += OnTabDragStarting;
        BusinessTabs.TabDroppedOutside += OnTabDroppedOutside;
        BusinessTabs.TabItemsSource = _openPages;

        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _clockTimer.Tick += (_, _) => _viewModel.UpdateClock(DateTime.Now);
        _clockTimer.Start();

        _toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        _toastTimer.Tick += (_, _) =>
        {
            _toastTimer.Stop();
            ToastHost.IsVisible = false;
        };

        Loaded += OnWindowLoaded;
        Opened += OnWindowOpened;
        Closed += OnWindowClosed;
        AddHandler(DragDrop.DragOverEvent, OnWindowDragOver);

        OpenPage(NavigationCatalog.Home.Key);
        UpdateShell();
    }

    internal TabManager Tabs => _tabs;

    internal RuntimeDiagnostics Diagnostics => _diagnostics;

    internal IVisitRepository Repository => _repository;

    internal MainWindowViewModel ShellViewModel => _viewModel;

    internal bool TabDragSafetyVerified => _tabDragSafetyVerified;

    internal bool NavCollapsed => _navCollapsed;

    internal double NavWidth => NavHost.Width;

    internal int PageHostChildCount => PageHost.Children.Count;

    internal void SetNavigationCollapsedForTest(bool collapsed) => SetNavigationCollapsed(collapsed);

    internal PageDocument? FindPage(PageKey key) => _tabs.Find(key) as PageDocument;

    internal PageDocument? ActivePage => _tabs.Active as PageDocument;

    internal void OpenPage(PageKey key)
    {
        try
        {
            var existing = _tabs.Find(key);
            if (existing is not null)
            {
                _tabs.Activate(existing);
                return;
            }

            var page = _registry.Create(key);
            page.ViewModel.CloseRequested += OnPageCloseRequested;
            page.ViewModel.PropertyChanged += OnPageViewModelPropertyChanged;
            _tabs.Open(page);
            _viewModel.RecordRecent(page);
            UpdateShell();
        }
        catch (Exception ex)
        {
            ShowToast($"无法打开页面：{ex.Message}");
            _diagnostics.RecordNote($"OpenPage failed {key.Value}: {ex.Message}");
        }
    }

    internal async Task<bool> TryCloseForTestAsync(PageDocument page, CloseDecision decision)
    {
        return await _tabs.TryCloseAsync(page, decision);
    }

    internal async Task<bool> CancelCloseThroughDialogForTestAsync(PageDocument page)
    {
        var closeTask = _tabs.TryCloseAsync(page);
        await Task.Delay(250);
        CompleteCloseDialog(CloseDecision.Cancel);
        return await closeTask;
    }

    internal void ActivatePage(PageKey key) => _tabs.Activate(key);

    internal async Task RunSelfTestIfRequestedAsync()
    {
        if (_selfTestOutput is null || _selfTestStarted)
            return;

        _selfTestStarted = true;
        var runner = new SelfTestRunner(this, _selfTestOutput);
        await runner.RunAsync();
    }

    private void OnWindowLoaded(object? sender, RoutedEventArgs e)
    {
        FitWindowToWorkingArea();
        ConfigureTabDragSafety();
        UpdateNavClasses(_tabs.Active?.Key.Kind);
        BusinessTabs.Focus();
    }

    private void FitWindowToWorkingArea()
    {
        var screen = Screens.ScreenFromWindow(this) ?? Screens.Primary;
        if (screen is null)
            return;

        var scale = RenderScaling > 0 ? RenderScaling : 1;
        var working = screen.WorkingArea;
        var maxWidth = Math.Max(MinWidth, working.Width / scale - 12);
        var maxHeight = Math.Max(MinHeight, working.Height / scale - 12);
        if (Width <= maxWidth && Height <= maxHeight)
            return;

        var targetWidth = Math.Min(Width, maxWidth);
        var targetHeight = Math.Min(Height, maxHeight);
        var left = working.X / scale + (working.Width / scale - targetWidth) / 2;
        var top = working.Y / scale + (working.Height / scale - targetHeight) / 2;
        Width = targetWidth;
        Height = targetHeight;
        Position = new PixelPoint(
            (int)Math.Round(left * scale),
            (int)Math.Round(top * scale));
    }

    private async void OnWindowOpened(object? sender, EventArgs e)
    {
        if (_selfTestOutput is not null)
            await RunSelfTestIfRequestedAsync();
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        _clockTimer.Stop();
        _toastTimer.Stop();
    }

    private void OnTabsPageOpened(object? sender, WorkspacePageEventArgs e)
    {
        if (e.Page is not PageDocument page)
            return;

        page.View.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        page.View.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        page.View.IsVisible = false;
        PageHost.Children.Add(page.View);
        _openPages.Add(page);
    }

    private void OnTabsPageReleasing(object? sender, WorkspacePageEventArgs e)
    {
        if (e.Page is not PageDocument page)
            return;

        _syncingTabs = true;
        try
        {
            var removed = PageHost.Children.Remove(page.View);
            _diagnostics.RecordNote(
                $"PageHost remove {page.Key.Value}: removed={removed}, remaining={PageHost.Children.Count}, viewParent={page.View.Parent?.GetType().Name ?? "null"}");
            _openPages.Remove(page);
            page.ViewModel.CloseRequested -= OnPageCloseRequested;
            page.ViewModel.PropertyChanged -= OnPageViewModelPropertyChanged;
        }
        finally
        {
            _syncingTabs = false;
        }
    }

    private void OnTabsPageClosed(object? sender, WorkspacePageEventArgs e)
    {
        UpdateShell();
    }

    private void OnTabsActiveChanged(object? sender, EventArgs e)
    {
        SyncActivePage();
        UpdateShell();
    }

    private void SyncActivePage()
    {
        var active = _tabs.Active as PageDocument;
        _syncingTabs = true;
        try
        {
            foreach (var child in PageHost.Children)
                child.IsVisible = active is not null && ReferenceEquals(child, active.View);

            if (active is not null)
                BusinessTabs.SelectedItem = active;
        }
        finally
        {
            _syncingTabs = false;
        }

        UpdateNavClasses(active?.Key.Kind);
    }

    private void OnTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_syncingTabs)
            return;

        if (BusinessTabs.SelectedItem is PageDocument page)
            _tabs.Activate(page);
        else if (BusinessTabs.SelectedItem is TabViewItem { Tag: IWorkspacePage workspacePage })
            _tabs.Activate(workspacePage);
    }

    private async void OnTabCloseRequested(object? sender, TabViewTabCloseRequestedEventArgs e)
    {
        var page = e.Item as PageDocument ?? e.Tab.Tag as PageDocument;
        if (page is null)
            return;

        await ClosePageAsync(page);
    }

    private async Task ClosePageAsync(PageDocument page)
    {
        if (!page.IsClosable || !_closingPages.Add(page.Key))
            return;

        try
        {
            await _tabs.TryCloseAsync(page);
        }
        finally
        {
            _closingPages.Remove(page.Key);
        }
    }
    private async void OnPageCloseRequested(object? sender, EventArgs e)
    {
        if (sender is not IWorkspacePageViewModel viewModel)
            return;

        var page = _tabs.Documents.OfType<PageDocument>()
            .FirstOrDefault(candidate => ReferenceEquals(candidate.ViewModel, viewModel));
        if (page is not null)
            await ClosePageAsync(page);
    }

    private void OnPageViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Tab headers bind directly to ViewModel.IsDirty; the shell only refreshes counters.
        UpdateShell();
    }

    private Task<CloseDecision> PromptCloseAsync(IWorkspacePage workspacePage)
    {
        if (workspacePage is not PageDocument page)
            return Task.FromResult(CloseDecision.Cancel);

        CloseDialogTitle.Text = "保存更改后关闭？";
        CloseDialogDetail.Text = page.Title;
        CloseDialogMessage.Text = page.Key.Kind == PageKind.VisitDetail
            ? "当前接诊详情包含未保存的草稿。保存会写入内存仓储；放弃会恢复到上次保存状态并关闭标签。"
            : "当前页面包含未保存的更改。请选择如何处理后再关闭标签。";
        CloseDialogHint.Text = "选择“取消”将保留当前标签、草稿和 WebView 状态。";
        CloseDialogOverlay.IsVisible = true;
        DialogSaveButton.Focus();

        _closeDialogCompletion = new TaskCompletionSource<CloseDecision>(TaskCreationOptions.RunContinuationsAsynchronously);
        return _closeDialogCompletion.Task;
    }

    private void CompleteCloseDialog(CloseDecision decision)
    {
        var completion = _closeDialogCompletion;
        _closeDialogCompletion = null;
        CloseDialogOverlay.IsVisible = false;
        completion?.TrySetResult(decision);
    }

    private void OnDialogCancelClick(object? sender, RoutedEventArgs e) => CompleteCloseDialog(CloseDecision.Cancel);

    private void OnDialogDiscardClick(object? sender, RoutedEventArgs e) => CompleteCloseDialog(CloseDecision.Discard);

    private void OnDialogSaveClick(object? sender, RoutedEventArgs e) => CompleteCloseDialog(CloseDecision.Save);

    private void OnNavHomeClick(object? sender, RoutedEventArgs e) => OpenPage(NavigationCatalog.Home.Key);

    private void OnNavVisitListClick(object? sender, RoutedEventArgs e) => OpenPage(NavigationCatalog.VisitList.Key);

    private void OnNavWebClick(object? sender, RoutedEventArgs e) => OpenPage(NavigationCatalog.WebDemo.Key);

    private void OnRecentPageClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: PageKey key })
            OpenPage(key);
    }

    private void OnToggleNavigation(object? sender, RoutedEventArgs e) =>
        SetNavigationCollapsed(!_navCollapsed);

    private void SetNavigationCollapsed(bool collapsed)
    {
        _navCollapsed = collapsed;
        NavHost.Width = collapsed ? 60 : 232;
        if (collapsed)
            Classes.Add("nav-collapsed");
        else
            Classes.Remove("nav-collapsed");
        ToolTip.SetTip(NavToggleButton, collapsed ? "展开导航" : "收起导航");
    }

    private void OnGlobalSearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            _viewModel.ClearSearch();
            e.Handled = true;
            return;
        }

        if (e.Key != Key.Enter || _viewModel.SearchResults.Count == 0)
            return;

        _viewModel.OpenSearchResult(_viewModel.SearchResults[0]);
        GlobalSearchBox.Focus();
        e.Handled = true;
    }

    private void OnSearchResultClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: SearchResultItem result })
        {
            _viewModel.OpenSearchResult(result);
            GlobalSearchBox.Focus();
        }
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (CloseDialogOverlay.IsVisible && e.Key == Key.Escape)
        {
            CompleteCloseDialog(CloseDecision.Cancel);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.K && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            GlobalSearchBox.Focus();
            GlobalSearchBox.SelectAll();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape && _viewModel.IsSearchOpen)
        {
            _viewModel.ClearSearch();
            e.Handled = true;
        }
    }

    private void OnWindowDragOver(object? sender, DragEventArgs e)
    {
        // Never accept tab tear-out or arbitrary external data. This handler also makes the
        // single-window document policy explicit at the shell boundary.
        e.DragEffects = DragDropEffects.None;
    }

    private void OnTabDragStarting(object? sender, TabViewTabDragStartingEventArgs e)
    {
        e.Cancel = true;
        _diagnostics.RecordNote("TabDragStarting cancelled: tear-out disabled");
    }

    private void OnTabDroppedOutside(object? sender, TabViewTabDroppedOutsideEventArgs e)
    {
        _diagnostics.RecordNote("TabDroppedOutside observed: no secondary window created");
        ShowToast("标签不支持拖出为独立窗口。");
    }

    private void ConfigureTabDragSafety()
    {
        DragDrop.SetAllowDrop(BusinessTabs, false);
        var lists = BusinessTabs.GetVisualDescendants().OfType<TabViewListView>().ToArray();
        foreach (var list in lists)
        {
            list.CanReorderItems = false;
            DragDrop.SetAllowDrop(list, false);
        }

        _tabDragSafetyVerified = !BusinessTabs.CanReorderTabs
                                 && !DragDrop.GetAllowDrop(BusinessTabs)
                                 && lists.Length > 0
                                 && lists.All(list => !list.CanReorderItems && !DragDrop.GetAllowDrop(list));
        _diagnostics.RecordNote(
            $"Tab drag safety verified={_tabDragSafetyVerified}; lists={lists.Length}; CanReorderTabs={BusinessTabs.CanReorderTabs}");
    }

    private void UpdateShell()
    {
        _viewModel.UpdateShellState(_tabs.Active, _tabs.Documents.Count, _diagnostics);
        _viewModel.WebStateText = GetWebStateText();
        if (_tabs.Active is PageDocument page)
            _viewModel.RecordRecent(page);
    }

    private string GetWebStateText()
    {
        if (_tabs.Active is not PageDocument page || page.View is not WebDemoView webView)
            return _diagnostics.WebAdapterCreated == 0 ? "WebView2 待初始化" : "WebView2 已按需加载";

        if (webView.HasLiveAdapter)
            return "WebView2 已连接";
        if (webView.AdapterDestroyedCount > 0)
            return "WebView2 已释放";
        return "WebView2 初始化中";
    }

    private void UpdateNavClasses(PageKind? kind)
    {
        SetClass(NavHome, "active", kind == PageKind.Home);
        SetClass(NavVisitList, "active", kind is PageKind.VisitList or PageKind.VisitDetail);
        SetClass(NavWeb, "active", kind == PageKind.WebDemo);
    }

    private static void SetClass(Control control, string className, bool active)
    {
        if (active)
            control.Classes.Add(className);
        else
            control.Classes.Remove(className);
    }

    private void OnDiagnosticsChanged(object? sender, EventArgs e)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(UpdateShell);
            return;
        }

        UpdateShell();
    }

    private void OnRepositoryChanged(object? sender, EventArgs e)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(UpdateShell);
            return;
        }

        UpdateShell();
    }

    private void ShowToast(string message)
    {
        ToastText.Text = message;
        ToastHost.IsVisible = true;
        _toastTimer.Stop();
        _toastTimer.Start();
    }
}
