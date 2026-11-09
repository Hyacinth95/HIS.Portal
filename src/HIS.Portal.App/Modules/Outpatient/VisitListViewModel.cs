using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HIS.Portal.Contracts;
using HIS.Portal.Navigation;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Demo;

namespace HIS.Portal.Modules.Outpatient;

public sealed partial class VisitListViewModel : PageViewModelBase
{
    private readonly IVisitRepository _repository;
    private readonly Action<PageKey> _navigate;

    public VisitListViewModel(IVisitRepository repository, Action<PageKey> navigate)
    {
        _repository = repository;
        _navigate = navigate;
        StatusOptions = ["全部状态", "待接诊", "已接诊", "已完成"];
        _selectedStatus = StatusOptions[0];
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        OpenVisitCommand = new RelayCommand<VisitRecord?>(OpenVisit);
        OpenSelectedCommand = new RelayCommand(OpenSelected, () => SelectedVisit is not null);
        ClearFiltersCommand = new RelayCommand(ClearFilters);
        Load();
        PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is nameof(SearchText) or nameof(SelectedStatus))
                ApplyFilter();
        };
    }

    public override string Title => "门诊接诊";

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedStatus = "全部状态";

    [ObservableProperty]
    private VisitRecord? _selectedVisit;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _resultSummary = string.Empty;

    [ObservableProperty]
    private string _statusSummary = string.Empty;

    [ObservableProperty]
    private string _lastRefreshText = "刚刚同步";

    public ObservableCollection<VisitRecord> VisibleVisits { get; } = [];

    public IReadOnlyList<string> StatusOptions { get; }

    public IAsyncRelayCommand RefreshCommand { get; }

    public IRelayCommand<VisitRecord?> OpenVisitCommand { get; }

    public IRelayCommand OpenSelectedCommand { get; }

    public IRelayCommand ClearFiltersCommand { get; }

    public async Task RefreshAsync()
    {
        await Task.Delay(120);
        Load();
        LastRefreshText = $"已刷新 {DateTime.Now:HH:mm:ss}";
    }

    public void OpenSelected()
    {
        OpenVisit(SelectedVisit);
    }

    public void OpenVisit(VisitRecord? visit)
    {
        if (visit is not null)
            _navigate(PageKey.ForVisit(visit.VisitNo));
    }

    public void ClearFilters()
    {
        SearchText = string.Empty;
        SelectedStatus = StatusOptions[0];
    }

    private void Load()
    {
        _allVisits = _repository.GetAll();
        ApplyFilter();
    }

    private IReadOnlyList<VisitRecord> _allVisits = [];

    private void ApplyFilter()
    {
        var selectedNo = SelectedVisit?.VisitNo;
        var query = (SearchText ?? string.Empty).Trim();
        var filtered = _allVisits.Where(visit =>
            (string.IsNullOrEmpty(query)
             || visit.VisitNo.Contains(query, StringComparison.OrdinalIgnoreCase)
             || visit.PatientName.Contains(query, StringComparison.OrdinalIgnoreCase)
             || visit.Department.Contains(query, StringComparison.OrdinalIgnoreCase)
             || visit.Doctor.Contains(query, StringComparison.OrdinalIgnoreCase))
            && (SelectedStatus == "全部状态" || visit.Status == SelectedStatus))
            .ToArray();

        // Update the observable collection in place instead of Clear()+Add(). A clear resets the
        // DataGrid scroll position and selection; the in-place diff keeps the viewport stable.
        for (var i = VisibleVisits.Count - 1; i >= 0; i--)
        {
            if (!filtered.Contains(VisibleVisits[i]))
                VisibleVisits.RemoveAt(i);
        }

        for (var i = 0; i < filtered.Length; i++)
        {
            if (i >= VisibleVisits.Count)
                VisibleVisits.Add(filtered[i]);
            else if (!ReferenceEquals(VisibleVisits[i], filtered[i]))
                VisibleVisits[i] = filtered[i];
        }

        // Restore selection by business id. If the record disappeared, clear the selection:
        // never auto-select (and therefore never auto-open) an unrelated first row.
        SelectedVisit = selectedNo is null
            ? null
            : VisibleVisits.FirstOrDefault(visit =>
                string.Equals(visit.VisitNo, selectedNo, StringComparison.OrdinalIgnoreCase));
        IsEmpty = VisibleVisits.Count == 0;
        ResultSummary = $"共 {_allVisits.Count} 条记录，当前显示 {VisibleVisits.Count} 条";
        var waiting = _allVisits.Count(visit => visit.Status == "待接诊");
        var completed = _allVisits.Count(visit => visit.Status == "已完成");
        StatusSummary = $"待接诊 {waiting} · 已完成 {completed}";
        OpenSelectedCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedVisitChanged(VisitRecord? value)
    {
        OpenSelectedCommand?.NotifyCanExecuteChanged();
    }
}
