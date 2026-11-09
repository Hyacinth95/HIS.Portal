using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HIS.Portal.Contracts;
using HIS.Portal.Navigation;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Demo;

namespace HIS.Portal.Modules.Home;

public sealed partial class HomeViewModel : PageViewModelBase
{
    private readonly IVisitRepository _repository;
    private readonly Action<PageKey> _navigate;

    public HomeViewModel(IVisitRepository repository, Action<PageKey> navigate)
    {
        _repository = repository;
        _navigate = navigate;

        var visits = repository.GetAll();
        TodayVisits = visits.Count;
        WaitingVisits = visits.Count(visit => visit.Status == "待接诊");
        CompletedVisits = visits.Count(visit => visit.Status == "已完成");
        PriorityVisits = visits.Count(visit => visit.Priority is "加急" or "优先");
        RecentVisits = new ObservableCollection<VisitRecord>(visits.Take(4));
        WaitingQueue = new ObservableCollection<VisitRecord>(
            visits.Where(visit => visit.Status == "待接诊").Take(4));
        NextWaitingText = WaitingQueue.FirstOrDefault() is { } next
            ? $"下一位：{next.PatientName} · {next.Department} · {next.AppointmentTime}"
            : "当前没有待接诊患者";
        TodayText = DateTime.Now.ToString("yyyy年M月d日 dddd");
        Greeting = BuildGreeting();
    }

    public override string Title => "工作台首页";

    public string Greeting { get; }

    public string TodayText { get; }

    public string NextWaitingText { get; }

    public string WaitingBadgeText => $"待接诊 {WaitingVisits}";

    public int TodayVisits { get; }

    public int WaitingVisits { get; }

    public int CompletedVisits { get; }

    public int PriorityVisits { get; }

    public ObservableCollection<VisitRecord> RecentVisits { get; }

    public ObservableCollection<VisitRecord> WaitingQueue { get; }

    [RelayCommand]
    private void OpenVisitList() => _navigate(PageKey.VisitList);

    [RelayCommand]
    private void OpenWebDemo() => _navigate(PageKey.WebDemo);

    [RelayCommand]
    private void OpenVisit(VisitRecord? visit)
    {
        if (visit is not null)
            _navigate(PageKey.ForVisit(visit.VisitNo));
    }

    [RelayCommand]
    private void OpenFirstWaitingVisit()
    {
        var visit = _repository.GetAll().FirstOrDefault(item => item.Status == "待接诊");
        if (visit is not null)
            _navigate(PageKey.ForVisit(visit.VisitNo));
    }

    private static string BuildGreeting()
    {
        var hour = DateTime.Now.Hour;
        return hour switch
        {
            < 6 => "夜深了，注意休息",
            < 12 => "早上好，陈晓医生",
            < 14 => "中午好，陈晓医生",
            < 18 => "下午好，陈晓医生",
            _ => "晚上好，陈晓医生",
        };
    }
}
