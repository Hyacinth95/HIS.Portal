using HIS.Portal.Contracts;
using HIS.Portal.Infrastructure.Demo;
using HIS.Portal.Modules.Home;
using HIS.Portal.Modules.Outpatient;
using HIS.Portal.Navigation;

namespace HIS.Portal.Tests;

public class VisitDomainTests
{
    [Fact]
    public void PageKey_IncludesKindAndNormalizedBusinessId()
    {
        var key = PageKey.ForVisit(" mz20261108-001 ");

        Assert.Equal(PageKind.VisitDetail, key.Kind);
        Assert.Equal("MZ20261108-001", key.BusinessId);
        Assert.Equal("VisitDetail:MZ20261108-001", key.Value);
        Assert.NotEqual(PageKey.ForVisit("MZ20261108-002"), key);
    }

    [Fact]
    public void Repository_ContainsThirtySyntheticVisitsAndReturnsDefensiveCopies()
    {
        IVisitRepository repository = new DemoVisitRepository();

        Assert.True(repository.IsSynthetic);
        Assert.Contains("合成数据", repository.DataSourceNotice, StringComparison.Ordinal);
        Assert.Equal(30, repository.Count);
        var original = repository.GetAll()[0];
        original.Notes = "直接修改返回对象";
        var reloaded = repository.Find(original.VisitNo);

        Assert.NotNull(reloaded);
        Assert.NotEqual("直接修改返回对象", reloaded.Notes);
    }

    [Fact]
    public async Task DetailDraft_DirtySaveAndDiscardWorkThroughInterface()
    {
        var repository = new DemoVisitRepository();
        var record = repository.GetAll()[0];
        var viewModel = new VisitDetailViewModel(record, repository);
        IWorkspacePageViewModel pageViewModel = viewModel;

        Assert.False(pageViewModel.IsDirty);
        viewModel.Notes = "新的接诊备注";
        viewModel.Diagnosis = "新的诊断";
        Assert.True(pageViewModel.IsDirty);

        var saved = await pageViewModel.SaveAsync();
        Assert.True(saved);
        Assert.False(pageViewModel.IsDirty);

        var savedRecord = repository.Find(record.VisitNo);
        Assert.NotNull(savedRecord);
        Assert.Equal("新的接诊备注", savedRecord.Notes);
        Assert.Equal("新的诊断", savedRecord.Diagnosis);

        viewModel.Notes = "放弃的草稿";
        Assert.True(viewModel.IsDirty);
        viewModel.DiscardChanges();

        Assert.False(viewModel.IsDirty);
        Assert.Equal("新的接诊备注", viewModel.Notes);
        var repoAfterDiscard = repository.Find(record.VisitNo);
        Assert.NotNull(repoAfterDiscard);
        Assert.Equal("新的接诊备注", repoAfterDiscard.Notes);
    }

    [Fact]
    public void VisitRecord_GenderAgeAndInitialsAreStable()
    {
        var record = new VisitRecord
        {
            VisitNo = "V1",
            PatientName = "林芷晴",
            Gender = "女",
            Age = 35,
            Department = "心血管内科",
            Doctor = "陈晓",
            Status = "待接诊",
            AppointmentTime = "08:30",
            Priority = "普通",
            ChiefComplaint = "胸闷",
            Diagnosis = "待完善",
            Notes = string.Empty,
        };

        Assert.Equal("林", record.Initials);
        Assert.Equal("女 · 35 岁", record.GenderAge);
        Assert.Equal("接诊", record.OpenActionLabel);
    }

    [Fact]
    public void VisitList_FilterClearsVanishedSelectionWithoutAutoSelectingFirstRow()
    {
        var repository = new DemoVisitRepository();
        PageKey? navigated = null;
        var viewModel = new VisitListViewModel(repository, key => navigated = key);
        viewModel.SelectedVisit = viewModel.VisibleVisits[1];

        viewModel.SearchText = "林芷晴";

        Assert.Single(viewModel.VisibleVisits);
        Assert.Equal("林芷晴", viewModel.VisibleVisits[0].PatientName);
        Assert.Null(viewModel.SelectedVisit);
        Assert.Null(navigated);

        viewModel.ClearFilters();

        Assert.Equal(30, viewModel.VisibleVisits.Count);
        Assert.Null(viewModel.SelectedVisit);
        Assert.Null(navigated);
    }

    [Fact]
    public void VisitList_FilterKeepsSelectionForRecordThatRemainsVisible()
    {
        var repository = new DemoVisitRepository();
        var viewModel = new VisitListViewModel(repository, _ => { });
        var first = viewModel.VisibleVisits[0];
        viewModel.SelectedVisit = first;

        viewModel.SearchText = "林芷晴";

        Assert.Single(viewModel.VisibleVisits);
        Assert.Same(first, viewModel.SelectedVisit);
        Assert.Equal(first.VisitNo, viewModel.SelectedVisit?.VisitNo);
    }

    [Fact]
    public void VisitList_OpenButtonAndKeyboardPathShareVisitPageKey()
    {
        var repository = new DemoVisitRepository();
        var navigated = new List<PageKey>();
        var viewModel = new VisitListViewModel(repository, navigated.Add);
        var record = viewModel.VisibleVisits[0];
        viewModel.SelectedVisit = record;

        viewModel.OpenSelected();
        viewModel.OpenVisit(record);

        Assert.Equal(2, navigated.Count);
        Assert.Equal(PageKey.ForVisit(record.VisitNo), navigated[0]);
        Assert.Equal(PageKey.ForVisit(record.VisitNo), navigated[1]);
    }

    [Fact]
    public void Home_ShowsWaitingQueueAndNextWaitingAction()
    {
        var repository = new DemoVisitRepository();
        var viewModel = new HomeViewModel(repository, _ => { });

        Assert.NotEmpty(viewModel.WaitingQueue);
        Assert.All(viewModel.WaitingQueue, visit => Assert.Equal("待接诊", visit.Status));
        Assert.Contains("下一位", viewModel.NextWaitingText);
        Assert.Equal($"待接诊 {viewModel.WaitingVisits}", viewModel.WaitingBadgeText);
    }
}
