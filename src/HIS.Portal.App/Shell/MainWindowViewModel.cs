using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HIS.Portal.Contracts;
using HIS.Portal.Navigation;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Demo;

namespace HIS.Portal.Shell;

public sealed class SearchResultItem
{
    public required string Title { get; init; }

    public required string Subtitle { get; init; }

    public required string Glyph { get; init; }

    public required PageKey Key { get; init; }
}

public sealed class RecentPageItem
{
    public required PageKey Key { get; init; }

    public required string Title { get; init; }

    public required string Subtitle { get; init; }

    public required string TimeText { get; init; }

    public string Glyph => Key.Kind switch
    {
        PageKind.Home => "\uE80F",
        PageKind.VisitList => "\uE8FD",
        PageKind.VisitDetail => "\uE77B",
        PageKind.WebDemo => "\uE774",
        _ => "\uE8A5",
    };
}

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IVisitRepository _repository;
    private Action<PageKey>? _navigate;

    public MainWindowViewModel(IVisitRepository repository)
    {
        _repository = repository;
        OpenSearchResultCommand = new RelayCommand<SearchResultItem?>(OpenSearchResult);
        PropertyChanged += OnShellPropertyChanged;
        ClockText = DateTime.Now.ToString("HH:mm:ss");
        DateText = DateTime.Now.ToString("yyyy年M月d日 dddd");
    }

    public ObservableCollection<SearchResultItem> SearchResults { get; } = [];

    public ObservableCollection<RecentPageItem> RecentPages { get; } = [];

    public IRelayCommand<SearchResultItem?> OpenSearchResultCommand { get; }

    public string DateText { get; }

    public string DataSourceNotice => _repository.DataSourceNotice;

    [ObservableProperty]
    private string _clockText;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isSearchOpen;

    [ObservableProperty]
    private bool _hasSearchResults;

    [ObservableProperty]
    private string _activePageTitle = "工作台首页";

    [ObservableProperty]
    private string _activePageKey = "Home:home";

    [ObservableProperty]
    private string _pageCountText = "1 个标签页";

    [ObservableProperty]
    private string _diagnosticsSummary = "页面创建 0 / 释放 0 · WebView 创建 0 / 销毁 0";

    [ObservableProperty]
    private string _webStateText = "WebView2 待初始化";

    [ObservableProperty]
    private string _statusHint = "演示模式 · 全部为合成数据";

    public void SetNavigator(Action<PageKey> navigate) => _navigate = navigate;

    public void UpdateClock(DateTime now)
    {
        ClockText = now.ToString("HH:mm:ss");
    }

    public void RecordRecent(IWorkspacePage page)
    {
        if (page.Key.Kind == PageKind.Home)
            return;

        var existing = RecentPages.FirstOrDefault(item => item.Key == page.Key);
        if (existing is not null)
            RecentPages.Remove(existing);

        RecentPages.Insert(0, new RecentPageItem
        {
            Key = page.Key,
            Title = page.Title,
            Subtitle = page.Key.Kind switch
            {
                PageKind.VisitList => "业务列表",
                PageKind.VisitDetail => page.Key.BusinessId,
                PageKind.WebDemo => "本地 HTML / NativeWebView",
                _ => page.Key.Value,
            },
            TimeText = DateTime.Now.ToString("HH:mm"),
        });

        while (RecentPages.Count > 6)
            RecentPages.RemoveAt(RecentPages.Count - 1);
    }

    public void UpdateShellState(IWorkspacePage? active, int pageCount, RuntimeDiagnostics diagnostics)
    {
        ActivePageTitle = active?.Title ?? "无活动页面";
        ActivePageKey = active?.Key.Value ?? "-";
        PageCountText = $"{pageCount} 个标签页";
        DiagnosticsSummary = diagnostics.Summary;
    }

    public void OpenSearchResult(SearchResultItem? result)
    {
        if (result is null)
            return;

        _navigate?.Invoke(result.Key);
        SearchText = string.Empty;
        IsSearchOpen = false;
    }

    public void CloseSearch()
    {
        IsSearchOpen = false;
    }

    public void ClearSearch()
    {
        SearchText = string.Empty;
        SearchResults.Clear();
        HasSearchResults = false;
        IsSearchOpen = false;
    }

    private void OnShellPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(SearchText))
            RebuildSearch();
    }

    private void RebuildSearch()
    {
        SearchResults.Clear();
        var query = (SearchText ?? string.Empty).Trim();
        if (query.Length == 0)
        {
            HasSearchResults = false;
            IsSearchOpen = false;
            return;
        }

        AddIfMatches(query, NavigationCatalog.Features[0]);
        AddIfMatches(query, NavigationCatalog.Features[1]);
        AddIfMatches(query, NavigationCatalog.Features[2]);

        foreach (var visit in _repository.GetAll()
                     .Where(visit => visit.VisitNo.Contains(query, StringComparison.OrdinalIgnoreCase)
                                     || visit.PatientName.Contains(query, StringComparison.OrdinalIgnoreCase)
                                     || visit.Department.Contains(query, StringComparison.OrdinalIgnoreCase)
                                     || visit.Doctor.Contains(query, StringComparison.OrdinalIgnoreCase))
                     .Take(7))
        {
            SearchResults.Add(new SearchResultItem
            {
                Title = $"{visit.PatientName} · 接诊详情",
                Subtitle = $"{visit.VisitNo} · {visit.Status} · {visit.Department}",
                Glyph = "\uE77B",
                Key = PageKey.ForVisit(visit.VisitNo),
            });
        }

        HasSearchResults = SearchResults.Count > 0;
        IsSearchOpen = true;
    }

    private void AddIfMatches(string query, NavigationFeature feature)
    {
        if (feature.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
            || feature.Subtitle.Contains(query, StringComparison.OrdinalIgnoreCase))
        {
            SearchResults.Add(new SearchResultItem
            {
                Title = feature.Title,
                Subtitle = feature.Subtitle,
                Glyph = feature.Glyph,
                Key = feature.Key,
            });
        }
    }
}
