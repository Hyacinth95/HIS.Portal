using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using HIS.Portal.Contracts;
using HIS.Portal.Navigation;
using HIS.Portal.Modules;
using HIS.Portal.Modules.Home;
using HIS.Portal.Modules.Outpatient;
using HIS.Portal.Modules.WebDemo;
using HIS.Portal.Shell;

namespace HIS.Portal.Diagnostics;

internal sealed class SelfTestRunner
{
    private readonly MainWindow _window;
    private readonly string _outputPath;
    private readonly List<CheckResult> _checks = [];
    private readonly List<string> _notes = [];

    public SelfTestRunner(MainWindow window, string outputPath)
    {
        _window = window;
        _outputPath = Path.GetFullPath(outputPath);
    }

    public async Task RunAsync()
    {
        var startedAt = DateTimeOffset.Now;
        var stopwatch = Stopwatch.StartNew();
        var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var primaryScreen = _window.Screens.ScreenFromWindow(_window) ?? _window.Screens.Primary;
        var initialClientSize = _window.ClientSize;
        var passed = false;

        try
        {
            await Task.Delay(1400);
            var home = _window.FindPage(PageKey.Home);
            Check("home-tab-initialized",
                home is not null && !home.IsClosable,
                $"首页存在且不可关闭：key={home?.Key.Value ?? "missing"}");
            Check("drag-tearout-disabled",
                _window.TabDragSafetyVerified,
                "CanReorderTabs=false，TabView 与内部 TabViewListView 的 AllowDrop 均为 false，TabDragStarting 取消拖拽。");
            Check("single-window-before",
                desktop is { Windows.Count: 1 },
                $"当前顶层窗口数：{desktop?.Windows.Count.ToString() ?? "unknown"}");

            _window.OpenPage(PageKey.VisitList);
            await PumpAsync(120);
            var list = _window.FindPage(PageKey.VisitList);
            Check("visit-list-opened",
                list?.ViewModel is VisitListViewModel,
                $"门诊接诊页存在：{list is not null}");

            if (list?.ViewModel is not VisitListViewModel listViewModel)
                throw new InvalidOperationException("门诊接诊页未创建，后续验收无法继续。");

            listViewModel.SearchText = "林芷晴";
            await PumpAsync(80);
            Check("visit-list-filter",
                listViewModel.VisibleVisits.Count == 1
                && listViewModel.VisibleVisits[0].PatientName == "林芷晴",
                $"搜索“林芷晴”结果数：{listViewModel.VisibleVisits.Count}");
            listViewModel.ClearFilters();
            await PumpAsync(80);
            Check("visit-list-filter-clear",
                listViewModel.VisibleVisits.Count == 30,
                $"清除筛选后记录数：{listViewModel.VisibleVisits.Count}");

            var documentCountBeforeSelection = _window.Tabs.Documents.Count;
            var pageCreatedBeforeSelection = _window.Diagnostics.PageCreated;
            listViewModel.SelectedVisit = listViewModel.VisibleVisits[1];
            await PumpAsync(80);
            Check("visit-list-selection-does-not-open",
                _window.Tabs.Documents.Count == documentCountBeforeSelection
                && _window.Diagnostics.PageCreated == pageCreatedBeforeSelection
                && ReferenceEquals(_window.ActivePage, list),
                $"仅选中一行后标签数={_window.Tabs.Documents.Count}，活动页仍为列表。");

            listViewModel.SearchText = "林芷晴";
            await PumpAsync(80);
            Check("visit-list-filter-clears-vanished-selection",
                listViewModel.VisibleVisits.Count == 1
                && listViewModel.SelectedVisit is null
                && _window.Tabs.Documents.Count == documentCountBeforeSelection,
                $"筛选“林芷晴”后选中项={listViewModel.SelectedVisit?.VisitNo ?? "null"}，未自动选中首行，也未打开页面。");

            listViewModel.ClearFilters();
            await PumpAsync(80);
            Check("visit-list-filter-clear-keeps-no-selection",
                listViewModel.VisibleVisits.Count == 30 && listViewModel.SelectedVisit is null,
                "清除筛选后仍保持无选择，不会自动选中第一条。");

            var filterKeptSelection = listViewModel.VisibleVisits[0];
            listViewModel.SelectedVisit = filterKeptSelection;
            listViewModel.SearchText = "林芷晴";
            await PumpAsync(80);
            Check("visit-list-filter-keeps-visible-selection",
                ReferenceEquals(listViewModel.SelectedVisit, filterKeptSelection)
                && listViewModel.VisibleVisits.Count == 1,
                $"记录仍在结果中时按就诊号恢复选中：{listViewModel.SelectedVisit?.VisitNo ?? "null"}。");
            listViewModel.ClearFilters();
            await PumpAsync(80);

            var records = _window.Repository.GetAll();
            var first = records[0];
            var second = records[1];
            var firstKey = PageKey.ForVisit(first.VisitNo);
            var secondKey = PageKey.ForVisit(second.VisitNo);

            var documentCountBeforeListOpen = _window.Tabs.Documents.Count;
            var pageCreatedBeforeListOpen = _window.Diagnostics.PageCreated;
            listViewModel.SelectedVisit = listViewModel.VisibleVisits[0];
            listViewModel.OpenSelected();
            await PumpAsync(150);
            Check("visit-list-open-selected-uses-visit-key",
                _window.FindPage(firstKey) is not null
                && _window.Tabs.Documents.Count == documentCountBeforeListOpen + 1
                && _window.Diagnostics.PageCreated == pageCreatedBeforeListOpen + 1,
                $"列表选中后打开详情：存在={_window.FindPage(firstKey) is not null}，标签数 {documentCountBeforeListOpen}->{_window.Tabs.Documents.Count}，PageCreated {pageCreatedBeforeListOpen}->{_window.Diagnostics.PageCreated}。");

            _window.ActivatePage(PageKey.VisitList);
            await PumpAsync(80);
            _window.OpenPage(firstKey);
            await PumpAsync(120);
            var detail = _window.FindPage(firstKey);
            if (detail?.ViewModel is not VisitDetailViewModel detailViewModel)
                throw new InvalidOperationException("首个接诊详情未创建。");

            var documentCountBeforeUnified = _window.Tabs.Documents.Count;
            var pageCreatedBeforeUnified = _window.Diagnostics.PageCreated;
            listViewModel.OpenVisit(first);
            listViewModel.OpenSelected();
            await PumpAsync(120);
            Check("visit-list-entry-paths-share-page-key",
                ReferenceEquals(_window.FindPage(firstKey), detail)
                && _window.Tabs.Documents.Count == documentCountBeforeUnified
                && _window.Diagnostics.PageCreated == pageCreatedBeforeUnified,
                "行内按钮与选中项打开最终复用同一个 PageKey，未创建第二个页面实例。");

            var documentCountBeforeDuplicate = _window.Tabs.Documents.Count;
            _window.OpenPage(firstKey);
            await PumpAsync(80);
            var duplicate = _window.FindPage(firstKey);
            Check("same-business-id-deduplicated",
                ReferenceEquals(detail, duplicate)
                && _window.Tabs.Documents.Count == documentCountBeforeDuplicate
                && _window.Tabs.Documents.Count(page => page.Key == firstKey) == 1,
                $"重复打开 {firstKey.Value} 后页实例相同，标签数保持 {_window.Tabs.Documents.Count}。");

            _window.OpenPage(secondKey);
            await PumpAsync(120);
            var detail2 = _window.FindPage(secondKey);
            Check("different-business-id-parallel",
                detail2 is not null
                && !ReferenceEquals(detail, detail2)
                && _window.Tabs.Documents.Count == documentCountBeforeDuplicate + 1,
                $"不同就诊号可并开：{firstKey.BusinessId} 与 {secondKey.BusinessId}。");

            var draftNote = $"自测草稿-{DateTime.Now:HHmmss}";
            var draftDiagnosis = "自测诊断：标签切换保留验证";
            detailViewModel.Notes = draftNote;
            detailViewModel.Diagnosis = draftDiagnosis;
            await PumpAsync(80);
            Check("draft-becomes-dirty",
                detailViewModel.IsDirty,
                "编辑备注与诊断后 IsDirty=true。");

            var viewReference = detail.View;
            var vmReference = detailViewModel;
            for (var i = 0; i < 10; i++)
            {
                _window.ActivatePage((i % 4) switch
                {
                    0 => PageKey.Home,
                    1 => PageKey.VisitList,
                    2 => firstKey,
                    _ => secondKey,
                });
                await PumpAsync(20);
            }

            _window.ActivatePage(firstKey);
            await PumpAsync(120);
            var afterTenSwitches = _window.FindPage(firstKey);
            Check("ten-switches-preserve-draft",
                ReferenceEquals(afterTenSwitches, detail)
                && ReferenceEquals(afterTenSwitches?.View, viewReference)
                && ReferenceEquals(afterTenSwitches?.ViewModel, vmReference)
                && detailViewModel.Notes == draftNote
                && detailViewModel.Diagnosis == draftDiagnosis
                && detailViewModel.IsDirty,
                $"十次切换后仍为同一 ViewModel/View，草稿与 Dirty 均保留（{draftNote}）。");

            if (detail.View is not VisitDetailView detailView)
                throw new InvalidOperationException("接诊详情视图类型不符。");

            var fixedIdentity = detailView.FixedIdentitySummary;
            detailView.DetailScrollViewer.ScrollToEnd();
            await PumpAsync(120);
            Check("detail-fixed-identity-and-save",
                fixedIdentity.Contains(first.PatientName, StringComparison.Ordinal)
                && fixedIdentity.Contains(first.VisitNo, StringComparison.Ordinal)
                && fixedIdentity.Contains(first.Department, StringComparison.Ordinal)
                && detailView.DetailScrollViewer.Offset.Y > 0
                && IsInsideWindow(detailView.SaveButton, _window),
                $"滚动至病历底部（offset={detailView.DetailScrollViewer.Offset.Y:0}）后固定摘要={fixedIdentity}，保存按钮仍在窗口内。");
            detailView.DetailScrollViewer.ScrollToHome();
            await PumpAsync(80);

            var originalWidth = _window.Width;
            var originalHeight = _window.Height;
            _window.Width = 1024;
            _window.Height = 540;
            await PumpAsync(350);
            if (Math.Abs(_window.ClientSize.Width - 1024) > 0.5)
            {
                _window.Width += 1024 - _window.ClientSize.Width;
                await PumpAsync(180);
            }

            if (Math.Abs(_window.ClientSize.Height - 540) > 0.5)
            {
                _window.Height += 540 - _window.ClientSize.Height;
                await PumpAsync(180);
            }

            var narrowClient = _window.ClientSize;
            var pageHost = _window.FindControl<Grid>("PageHost");
            Check("narrow-client-size-1024x540",
                Math.Abs(narrowClient.Width - 1024) < 0.5
                && Math.Abs(narrowClient.Height - 540) < 0.5
                && pageHost is not null
                && !_window.NavCollapsed
                && Math.Abs(_window.NavWidth - 232) < 0.5,
                $"ClientSize={narrowClient.Width:0}x{narrowClient.Height:0}，PageHost={pageHost?.Bounds.Width:0}x{pageHost?.Bounds.Height:0}，导航展开={!_window.NavCollapsed}，导航宽={_window.NavWidth:0}。");

            _window.ActivatePage(PageKey.VisitList);
            await PumpAsync(320);
            var listView = list.View as VisitListView
                           ?? throw new InvalidOperationException("接诊列表视图类型不符。");
            await PumpAsync(100);
            var listColumns = listView.VisitDataGrid.Columns;
            var compactOrder = string.Join(
                ",",
                listColumns
                    .OrderBy(column => column.DisplayIndex)
                    .Where(column => column.IsVisible)
                    .Select(column => column.Header?.ToString()));
            var keyColumns = listColumns[0].IsVisible && listColumns[0].ActualWidth >= 80
                             && listColumns[2].IsVisible && listColumns[2].ActualWidth >= 100
                             && listColumns[6].IsVisible && listColumns[6].ActualWidth >= 80
                             && listColumns[7].IsVisible && listColumns[7].ActualWidth >= 90
                             && listColumns[1].IsVisible && listColumns[1].ActualWidth >= 165
                             && listColumns[4].IsVisible && listColumns[4].ActualWidth >= 110;
            var hiddenColumns = !listColumns[3].IsVisible
                                && !listColumns[5].IsVisible
                                && !listColumns[8].IsVisible
                                && !listColumns[9].IsVisible;
            var noHorizontalScroll = listView.VisitDataGrid.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled
                                     && listView.VisitDataGrid.Columns
                                         .Where(column => column.IsVisible)
                                         .Sum(column => column.ActualWidth)
                                     <= listView.VisitDataGrid.Bounds.Width + 1;
            var keyHeadersInside = new[] { "操作", "患者", "状态", "优先级", "就诊号", "就诊科室" }
                .All(header => IsHeaderFullyInside(listView.VisitDataGrid, header));
            Check("narrow-list-key-columns",
                listView.IsCompact
                && compactOrder == "操作,患者,状态,优先级,就诊号,就诊科室"
                && keyColumns
                && hiddenColumns
                && noHorizontalScroll
                && keyHeadersInside
                && listView.VisitDataGrid.Bounds.Height > 80,
                $"Client={narrowClient.Width:0}x{narrowClient.Height:0}，compact={listView.IsCompact}，列={compactOrder}，关键列宽={listColumns[0].ActualWidth:0}/{listColumns[2].ActualWidth:0}/{listColumns[6].ActualWidth:0}/{listColumns[7].ActualWidth:0}/{listColumns[1].ActualWidth:0}/{listColumns[4].ActualWidth:0}，隐藏={hiddenColumns}，无横滚={noHorizontalScroll}（hvis={listView.VisitDataGrid.HorizontalScrollBarVisibility}），表头在网格内={keyHeadersInside}。");
            CaptureScreenshot("02-narrow-visit-list");

            _window.ActivatePage(firstKey);
            await PumpAsync(300);
            var chiefComplaintBox = detailView.ChiefComplaintBoxControl;
            var detailScroll = detailView.DetailScrollViewer;
            detailScroll.ScrollToHome();
            await PumpAsync(100);
            var boxOrigin = chiefComplaintBox.TranslatePoint(new Point(0, 0), detailScroll);
            var boxInsideViewport = boxOrigin is { } boxPoint
                                    && boxPoint.X >= -1
                                    && boxPoint.Y >= -1
                                    && boxPoint.X + chiefComplaintBox.Bounds.Width <= detailScroll.Bounds.Width + 1
                                    && boxPoint.Y + chiefComplaintBox.Bounds.Height <= detailScroll.Bounds.Height + 1;
            var compactDetail = detailView.IsCompact
                                && !detailView.IsBasicInfoExpanded
                                && !detailView.BasicInfoCardControl.IsEffectivelyVisible
                                && detailView.CompactBasicInfoSummaryControl.IsEffectivelyVisible;
            Check("narrow-detail-first-screen-edit",
                compactDetail
                && boxInsideViewport
                && chiefComplaintBox.Bounds.Height >= 60
                && Math.Abs(detailScroll.Offset.Y) < 0.5
                && IsInsideWindow(detailView.SaveButton, _window)
                && IsInsideWindow(detailView.DetailCloseButton, _window)
                && detailView.FixedIdentitySummary.Contains(first.PatientName, StringComparison.Ordinal)
                && detailView.FixedIdentitySummary.Contains(first.VisitNo, StringComparison.Ordinal),
                $"compact={detailView.IsCompact}，资料收起={!detailView.IsBasicInfoExpanded}，主诉框={chiefComplaintBox.Bounds.Width:0}x{chiefComplaintBox.Bounds.Height:0}，位于滚动视口={boxInsideViewport}，scrollY={detailScroll.Offset.Y:0}，身份={detailView.FixedIdentitySummary}，保存/关闭在窗口内。");
            CaptureScreenshot("03-narrow-detail");

            detailView.BasicInfoToggleButtonControl.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            await PumpAsync(120);
            var basicInfoTexts = detailView.BasicInfoCardControl
                .GetVisualDescendants()
                .OfType<TextBlock>()
                .Count(text => !string.IsNullOrWhiteSpace(text.Text));
            var basicInfoExpanded = detailView.IsBasicInfoExpanded
                                    && detailView.BasicInfoCardControl.IsEffectivelyVisible
                                    && basicInfoTexts >= 10
                                    && ReferenceEquals(detailView.ChiefComplaintBoxControl, chiefComplaintBox);
            detailView.BasicInfoToggleButtonControl.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            await PumpAsync(100);
            Check("narrow-basic-info-expand",
                basicInfoExpanded
                && !detailView.IsBasicInfoExpanded
                && !detailView.BasicInfoCardControl.IsEffectivelyVisible
                && ReferenceEquals(detailView.ChiefComplaintBoxControl, chiefComplaintBox),
                $"展开资料后原字段可见={basicInfoExpanded}（文本块={basicInfoTexts}），再次收起={!detailView.IsBasicInfoExpanded}，主诉控件同一实例。");

            var narrowDetailViewModel = detail.ViewModel as VisitDetailViewModel
                                  ?? throw new InvalidOperationException("接诊详情 ViewModel 类型不符。");
            var draftBeforeResize = narrowDetailViewModel.ChiefComplaint;
            var detailBeforeResize = detailView;
            var chiefBeforeResize = chiefComplaintBox;
            var narrowListViewModel = list.ViewModel as VisitListViewModel
                                ?? throw new InvalidOperationException("接诊列表 ViewModel 类型不符。");
            narrowListViewModel.SelectedVisit = narrowListViewModel.VisibleVisits[0];
            await PumpAsync(100);
            var selectedBeforeResize = narrowListViewModel.SelectedVisit;

            _window.Width = 1440;
            _window.Height = 900;
            await PumpAsync(420);
            _window.ActivatePage(PageKey.VisitList);
            await PumpAsync(200);
            var wideListOrder = string.Join(
                ",",
                listColumns
                    .OrderBy(column => column.DisplayIndex)
                    .Where(column => column.IsVisible)
                    .Select(column => column.Header?.ToString()));
            var wideList = !listView.IsCompact
                           && listColumns.All(column => column.IsVisible)
                           && wideListOrder == "操作,就诊号,患者,性别 / 年龄,就诊科室,接诊医生,状态,优先级,预约时间,主诉摘要";
            _window.ActivatePage(firstKey);
            await PumpAsync(250);
            var wideDetail = !detailView.IsCompact
                             && !detailView.IsSidebarStacked
                             && detailView.IsBasicInfoExpanded
                             && detailView.BasicInfoCardControl.IsEffectivelyVisible
                             && Math.Abs(detailView.SidebarColumnWidth - 310) < 0.5;
            Check("wide-layout-restored",
                wideList && wideDetail,
                $"1440x900：列表宽屏={wideList}，列={wideListOrder}；详情双栏={wideDetail}，侧栏宽={detailView.SidebarColumnWidth:0}。");
            CaptureScreenshot("04-wide-detail");

            _window.Width = 1024;
            _window.Height = 540;
            await PumpAsync(420);
            _window.ActivatePage(PageKey.VisitList);
            await PumpAsync(180);
            var resizedListCompact = listView.IsCompact
                                     && listView.VisitDataGrid.Columns[9].IsVisible == false;
            _window.ActivatePage(firstKey);
            await PumpAsync(180);
            Check("narrow-resize-preserves-instance-draft",
                resizedListCompact
                && detailView.IsCompact
                && !detailView.IsBasicInfoExpanded
                && ReferenceEquals(detailView, detailBeforeResize)
                && ReferenceEquals(narrowDetailViewModel, detail.ViewModel)
                && ReferenceEquals(detailView.ChiefComplaintBoxControl, chiefBeforeResize)
                && narrowDetailViewModel.ChiefComplaint == draftBeforeResize
                && detailViewModel.IsDirty
                && ReferenceEquals(narrowListViewModel.SelectedVisit, selectedBeforeResize)
                && narrowListViewModel.SelectedVisit is not null,
                $"回到 1024x540：列表 compact={resizedListCompact}，选中保持={ReferenceEquals(narrowListViewModel.SelectedVisit, selectedBeforeResize)}，详情 compact={detailView.IsCompact}，同一 View/VM/TextBox=True，草稿='{draftBeforeResize}'，Dirty={narrowDetailViewModel.IsDirty}。");

            _window.ActivatePage(PageKey.VisitList);
            await PumpAsync(200);
            var departmentBeforeNav = listView.VisitDataGrid.Columns[4].ActualWidth;
            var listBeforeNav = listView;
            _window.SetNavigationCollapsedForTest(true);
            await PumpAsync(320);
            var departmentCollapsedNav = listView.VisitDataGrid.Columns[4].ActualWidth;
            var navListSame = ReferenceEquals(_window.FindPage(PageKey.VisitList)?.View, listBeforeNav)
                              && listView.IsCompact;
            _window.SetNavigationCollapsedForTest(false);
            await PumpAsync(320);
            var departmentExpandedNav = listView.VisitDataGrid.Columns[4].ActualWidth;
            Check("narrow-nav-width-reflow",
                navListSame
                && departmentCollapsedNav > departmentBeforeNav + 20
                && Math.Abs(departmentExpandedNav - departmentBeforeNav) < 3,
                $"侧栏折叠后科室列 {departmentBeforeNav:0}→{departmentCollapsedNav:0}，展开后 {departmentExpandedNav:0}；同一列表实例={navListSame}。");

            listView.UpdateLayoutForTest(829, 421);
            var listStaysCompact = listView.IsCompact;
            listView.UpdateLayoutForTest(831, 421);
            var listEntersWide = !listView.IsCompact;
            listView.UpdateLayoutForTest(830, 421);
            var listStaysWide = !listView.IsCompact;
            listView.UpdateLayoutForTest(779, 421);
            var listEntersCompact = listView.IsCompact;
            listView.UpdateLayoutForTest(900, 379);
            var listHeightCompact = listView.IsCompact;
            listView.UpdateLayoutForTest(900, 421);
            var listHeightWide = !listView.IsCompact;
            listView.UpdateLayoutForTest(900, 419);
            var listHeightStaysWide = !listView.IsCompact;
            listView.UpdateLayoutForTest(900, 379);
            var listHeightBackCompact = listView.IsCompact;
            listView.UpdateLayoutForTest(1024, 540);

            _window.ActivatePage(firstKey);
            await PumpAsync(150);
            detailView.UpdateLayoutForTest(999, 700);
            var detailStaysCompact = detailView.IsCompact;
            detailView.UpdateLayoutForTest(1000, 560);
            var detailEntersWide = !detailView.IsCompact;
            detailView.UpdateLayoutForTest(999, 700);
            var detailStaysWide = !detailView.IsCompact;
            detailView.UpdateLayoutForTest(979, 700);
            var detailEntersCompact = detailView.IsCompact;
            detailView.UpdateLayoutForTest(1100, 519);
            var detailHeightCompact = detailView.IsCompact;
            detailView.UpdateLayoutForTest(1100, 561);
            var detailHeightWide = !detailView.IsCompact;
            detailView.UpdateLayoutForTest(1100, 559);
            var detailHeightStaysWide = !detailView.IsCompact;
            detailView.UpdateLayoutForTest(1100, 519);
            var detailHeightBackCompact = detailView.IsCompact;
            detailView.UpdateLayoutForTest(1024, 540);
            Check("narrow-threshold-hysteresis",
                listStaysCompact && listEntersWide && listStaysWide && listEntersCompact
                && listHeightCompact && listHeightWide && listHeightStaysWide && listHeightBackCompact
                && detailStaysCompact && detailEntersWide && detailStaysWide && detailEntersCompact
                && detailHeightCompact && detailHeightWide && detailHeightStaysWide && detailHeightBackCompact,
                $"列表宽 829 保持紧凑={listStaysCompact}，831 宽={listEntersWide}，830 宽={listStaysWide}，779 紧凑={listEntersCompact}；列表高 379 紧凑={listHeightCompact}，421 宽={listHeightWide}，419 宽={listHeightStaysWide}，379 紧凑={listHeightBackCompact}；详情宽 999 紧凑={detailStaysCompact}，1000 宽={detailEntersWide}，999 宽={detailStaysWide}，979 紧凑={detailEntersCompact}；详情高 519 紧凑={detailHeightCompact}，561 宽={detailHeightWide}，559 宽={detailHeightStaysWide}，519 紧凑={detailHeightBackCompact}。");

            var detailSaveInside = IsInsideWindow(detailView.SaveButton, _window);
            var detailCloseInside = IsInsideWindow(detailView.DetailCloseButton, _window);
            var detailStacked = detailView.IsSidebarStacked;
            var navVisitInside = IsInsideWindow(
                _window.FindControl<Button>("NavVisitList") ?? throw new InvalidOperationException("主导航按钮缺失。"),
                _window);
            Check("narrow-window-critical-actions",
                detailStacked && detailSaveInside && detailCloseInside && navVisitInside
                && listView.VisitDataGrid.Bounds.Height > 80,
                $"1024x540 下详情侧栏下移={detailStacked}，保存={detailSaveInside}，关闭={detailCloseInside}，导航={navVisitInside}，列表高度={listView.VisitDataGrid.Bounds.Height:0}。");

            var documentCountBeforeNav = _window.Tabs.Documents.Count;
            _window.SetNavigationCollapsedForTest(true);
            await PumpAsync(220);
            var favoriteVisit = _window.FindControl<Button>("FavoriteVisitButton");
            var recentPages = _window.FindControl<ItemsControl>("RecentPagesList");
            var navHome = _window.FindControl<Button>("NavHome");
            var visitCountBadge = _window.FindControl<Border>("VisitCountBadge");
            var webLiveBadge = _window.FindControl<Border>("WebLiveBadge");
            var collapsedWidth = _window.NavWidth;
            var collapsedNavState = _window.NavCollapsed;
            var collapsedFavoriteVisible = favoriteVisit?.IsVisible;
            var collapsedRecentVisible = recentPages?.IsVisible;
            var collapsedHomeVisible = navHome?.IsVisible;
            var collapsedVisitBadgeVisible = visitCountBadge?.IsVisible;
            var collapsedWebBadgeVisible = webLiveBadge?.IsVisible;
            var collapsedNavOk = _window.NavCollapsed
                                 && Math.Abs(collapsedWidth - 60) < 0.5
                                 && favoriteVisit is { IsVisible: false }
                                 && recentPages is { IsVisible: false }
                                 && navHome is { IsVisible: true }
                                 && visitCountBadge is { IsVisible: false }
                                 && webLiveBadge is { IsVisible: false }
                                 && _window.Tabs.Documents.Count == documentCountBeforeNav;
            _window.SetNavigationCollapsedForTest(false);
            await PumpAsync(220);
            Check("nav-collapse-hides-auxiliary",
                collapsedNavOk,
                $"折叠后状态={collapsedNavState}，宽度={collapsedWidth:0}，收藏可见={collapsedFavoriteVisible}，最近打开可见={collapsedRecentVisible}，主导航可见={collapsedHomeVisible}，数字徽标可见={collapsedVisitBadgeVisible}，LIVE 徽标可见={collapsedWebBadgeVisible}。");
            Check("nav-expand-restores",
                !_window.NavCollapsed
                && Math.Abs(_window.NavWidth - 232) < 0.5
                && favoriteVisit is { IsVisible: true }
                && recentPages is { IsVisible: true }
                && visitCountBadge is { IsVisible: true }
                && webLiveBadge is { IsVisible: true }
                && _window.Tabs.Documents.Count == documentCountBeforeNav,
                $"展开后宽度={_window.NavWidth:0}，收藏、最近打开与徽标恢复，标签数保持 {_window.Tabs.Documents.Count}。");

            _window.Width = originalWidth;
            _window.Height = originalHeight;
            await PumpAsync(300);
            _window.ActivatePage(PageKey.Home);
            await PumpAsync(280);
            var homeView = _window.FindPage(PageKey.Home)?.View as HomeView
                           ?? throw new InvalidOperationException("首页视图缺失。");
            var nextWaitingButton = homeView.FindControl<Button>("NextWaitingButton");
            var waitingQueue = homeView.FindControl<ItemsControl>("WaitingQueueList");
            Check("home-first-screen-next-waiting",
                nextWaitingButton is not null
                && IsInsideWindow(nextWaitingButton, _window)
                && waitingQueue is { ItemCount: > 0 },
                $"下一位患者按钮位于首屏={nextWaitingButton is not null && IsInsideWindow(nextWaitingButton, _window)}，待处理患者条目={waitingQueue?.ItemCount ?? 0}。");
            CaptureScreenshot("01-home-wide");

            _window.ActivatePage(firstKey);
            await PumpAsync(220);

            var canceled = await _window.CancelCloseThroughDialogForTestAsync(detail);
            await PumpAsync(100);
            Check("cancel-close-preserves-draft",
                !canceled
                && ReferenceEquals(_window.FindPage(firstKey), detail)
                && detailViewModel.Notes == draftNote
                && detailViewModel.IsDirty,
                $"canceled={canceled}; pagePresent={_window.FindPage(firstKey) is not null}; sameInstance={ReferenceEquals(_window.FindPage(firstKey), detail)}; notesMatch={detailViewModel.Notes == draftNote}; dirty={detailViewModel.IsDirty}; actualNotes={detailViewModel.Notes}");

            var savedAndClosed = await _window.TryCloseForTestAsync(detail, CloseDecision.Save);
            await PumpAsync(120);
            var savedRecord = _window.Repository.Find(first.VisitNo);
            Check("save-close-updates-memory",
                savedAndClosed
                && _window.FindPage(firstKey) is null
                && savedRecord?.Notes == draftNote
                && savedRecord?.Diagnosis == draftDiagnosis,
                $"closed={savedAndClosed}; pagePresent={_window.FindPage(firstKey) is not null}; repoNotes={savedRecord?.Notes ?? "null"}; expected={draftNote}; repoDiagnosis={savedRecord?.Diagnosis ?? "null"}");

            _window.OpenPage(firstKey);
            await PumpAsync(120);
            var reopened = _window.FindPage(firstKey);
            var reopenedViewModel = reopened?.ViewModel as VisitDetailViewModel;
            Check("reopen-detail-new-instance",
                reopened is not null
                && !ReferenceEquals(reopened, detail)
                && reopenedViewModel is not null
                && !reopenedViewModel.IsDirty
                && reopenedViewModel.Notes == draftNote,
                "关闭后重新打开按就诊号重建 View，读取到保存后的草稿且 Dirty=false。");

            if (reopened is null || reopenedViewModel is null)
                throw new InvalidOperationException("重开后的接诊详情未创建。");

            reopenedViewModel.Notes = "放弃测试：此内容不应写入仓储";
            await PumpAsync(60);
            var discardClosed = await _window.TryCloseForTestAsync(reopened, CloseDecision.Discard);
            await PumpAsync(120);
            var afterDiscard = _window.Repository.Find(first.VisitNo);
            Check("discard-close-keeps-saved-data",
                discardClosed
                && _window.FindPage(firstKey) is null
                && afterDiscard?.Notes == draftNote,
                "放弃更改并关闭后，仓储中仍是上一次保存的内容。");

            if (detail2 is not null)
            {
                await _window.TryCloseForTestAsync(detail2, CloseDecision.Discard);
                await PumpAsync(100);
            }

            _window.OpenPage(PageKey.WebDemo);
            await PumpAsync(220);
            var webPage = _window.FindPage(PageKey.WebDemo);
            var webView = webPage?.View as WebDemoView;
            Check("web-page-created",
                webPage is not null && webView is not null,
                "检查申请页已创建官方 NativeWebView。");

            if (webPage is null || webView is null)
                throw new InvalidOperationException("WebView 页面未创建。");

            await webView.WaitForReadyAsync(TimeSpan.FromSeconds(45));
            var bridgeReady = await webView.ProbeBridgeAsync();
            Check("real-webview-ready",
                webView.HasLiveAdapter && bridgeReady,
                $"适配器={webView.AdapterDescription}; NativeHandle={webView.NativeHandle?.ToString() ?? "null"}; JS bridge={bridgeReady}");
            CaptureScreenshot("05-web-demo");

            var initialState = await webView.ReadStateAsync()
                               ?? throw new InvalidOperationException("无法读取初始 DOM 状态。");
            var webNote = $"跨标签保留-{DateTime.Now:HHmmss}";
            await webView.ExecuteScriptAsync(
                $"document.getElementById('note').value={JsonSerializer.Serialize(webNote)};" +
                "document.getElementById('note').dispatchEvent(new Event('input',{bubbles:true}));" +
                "'ok'");
            await webView.IncrementCounterAsync();
            await webView.IncrementCounterAsync();
            await webView.IncrementCounterAsync();
            var beforeSwitch = await webView.ReadStateAsync()
                               ?? throw new InvalidOperationException("无法读取切换前 DOM 状态。");
            Check("webview-dom-editable",
                beforeSwitch.Note == webNote && beforeSwitch.Counter == 3,
                $"DOM 输入={beforeSwitch.Note}（期望 {webNote}），计数={beforeSwitch.Counter}，会话={beforeSwitch.SessionId}。");

            var diagnosticsDefaultCollapsed = !webView.DiagnosticsOpen;
            var defaultCollapsedVisual = !webView.DiagnosticsPanelVisible
                                         && Math.Abs(webView.DiagnosticsColumnWidth) < 0.5
                                         && webView.DiagnosticsButtonText == "展开诊断";
            var adapterBeforeDiagnostics = webView.AdapterCreatedCount;
            var navigationStartedBeforeDiagnostics = webView.NavigationStartedCount;
            for (var i = 0; i < 20; i++)
            {
                webView.SetDiagnosticsOpenForTest(true);
                await PumpAsync(15);
                webView.SetDiagnosticsOpenForTest(false);
                await PumpAsync(15);
            }

            var afterDiagnostics = await webView.ReadStateAsync();
            var closedVisual = !webView.DiagnosticsPanelVisible
                               && Math.Abs(webView.DiagnosticsColumnWidth) < 0.5
                               && webView.DiagnosticsButtonText == "展开诊断";
            Check("webview-diagnostics-toggle-20",
                diagnosticsDefaultCollapsed
                && defaultCollapsedVisual
                && !webView.DiagnosticsOpen
                && closedVisual
                && webView.HasLiveAdapter
                && webView.AdapterCreatedCount == adapterBeforeDiagnostics
                && webView.NavigationStartedCount == navigationStartedBeforeDiagnostics
                && afterDiagnostics is not null
                && afterDiagnostics.Note == webNote
                && afterDiagnostics.Counter == beforeSwitch.Counter
                && afterDiagnostics.SessionId == beforeSwitch.SessionId,
                $"默认收起={diagnosticsDefaultCollapsed}（XAML 列宽 0/面板隐藏/按钮“展开诊断”={defaultCollapsedVisual}）；从默认收起执行 20 次完整展开/收起后仍收起={!webView.DiagnosticsOpen}（视觉一致={closedVisual}）；AdapterCreated={webView.AdapterCreatedCount}（前 {adapterBeforeDiagnostics}），NavigationStarted={webView.NavigationStartedCount}（前 {navigationStartedBeforeDiagnostics}），DOM 与会话保留。");

            webView.SetDiagnosticsOpenForTest(true);
            await PumpAsync(80);
            var openVisual = webView.DiagnosticsPanelVisible
                             && Math.Abs(webView.DiagnosticsColumnWidth - 260) < 0.5
                             && webView.DiagnosticsButtonText == "收起诊断";
            _window.ActivatePage(PageKey.Home);
            await PumpAsync(50);
            _window.ActivatePage(PageKey.WebDemo);
            await PumpAsync(120);
            var openChoicePreserved = webView.DiagnosticsOpen;
            webView.SetDiagnosticsOpenForTest(false);
            await PumpAsync(80);
            var closedVisualAfterSwitch = !webView.DiagnosticsPanelVisible
                                          && Math.Abs(webView.DiagnosticsColumnWidth) < 0.5
                                          && webView.DiagnosticsButtonText == "展开诊断";
            _window.ActivatePage(PageKey.Home);
            await PumpAsync(50);
            _window.ActivatePage(PageKey.WebDemo);
            await PumpAsync(120);
            Check("webview-diagnostics-choice-preserved-on-tab-switch",
                openVisual
                && openChoicePreserved
                && !webView.DiagnosticsOpen
                && closedVisualAfterSwitch
                && webView.AdapterCreatedCount == adapterBeforeDiagnostics
                && webView.NavigationStartedCount == navigationStartedBeforeDiagnostics,
                $"展开视觉=列宽260/面板显示/按钮“收起诊断”={openVisual}；展开后切换标签仍展开={openChoicePreserved}；收起后切换标签仍收起={!webView.DiagnosticsOpen}（视觉一致={closedVisualAfterSwitch}）；Adapter/导航未增加。");

            var sessionBefore = beforeSwitch.SessionId;
            var webViewReference = webView;
            var adapterCreatedBefore = webView.AdapterCreatedCount;
            var nativeHandleBefore = webView.NativeHandle;
            for (var i = 0; i < 100; i++)
            {
                _window.ActivatePage((i % 3) switch
                {
                    0 => PageKey.WebDemo,
                    1 => PageKey.Home,
                    _ => PageKey.VisitList,
                });
                await PumpAsync(i % 10 == 0 ? 12 : 3);
            }

            _window.ActivatePage(PageKey.WebDemo);
            await PumpAsync(350);
            var afterSwitch = await webView.ReadStateAsync();
            Check("webview-100-switch-instance",
                ReferenceEquals(_window.FindPage(PageKey.WebDemo), webPage)
                && ReferenceEquals(webPage.View, webViewReference)
                && ReferenceEquals(webView, webViewReference)
                && webView.HasLiveAdapter
                && webView.AdapterCreatedCount == adapterCreatedBefore,
                $"100 次切换后仍是同一 NativeWebView 实例，适配器创建次数={webView.AdapterCreatedCount}，NativeHandle={nativeHandleBefore}。");
            Check("webview-100-switch-dom-state",
                afterSwitch is not null
                && afterSwitch.Note == webNote
                && afterSwitch.Counter == beforeSwitch.Counter
                && afterSwitch.SessionId == sessionBefore,
                $"DOM 输入、计数 {afterSwitch?.Counter} 与 JS 会话 {afterSwitch?.SessionId} 均保持不变。");

            var webClosed = await _window.TryCloseForTestAsync(webPage, CloseDecision.Discard);
            var adapterDestroyed = await WaitUntilAsync(
                () => webView.AdapterDestroyedCount > 0,
                TimeSpan.FromSeconds(8));
            Check("webview-close-releases-native",
                webClosed && adapterDestroyed,
                $"closed={webClosed}; AdapterDestroyed={webView.AdapterDestroyedCount}; viewParent={webView.Parent?.GetType().Name ?? "null"}; pageHostChildren={_window.PageHostChildCount}; pageReleased={webPage.IsReleased}");

            _window.OpenPage(PageKey.WebDemo);
            await PumpAsync(220);
            var webPage2 = _window.FindPage(PageKey.WebDemo);
            var webView2 = webPage2?.View as WebDemoView;
            if (webView2 is null)
                throw new InvalidOperationException("重开的 WebView 页面未创建。");

            await webView2.WaitForReadyAsync(TimeSpan.FromSeconds(45));
            var state2 = await webView2.ReadStateAsync()
                         ?? throw new InvalidOperationException("重开 WebView 后无法读取 DOM 状态。");
            Check("webview-reopen-new-instance",
                !ReferenceEquals(webView2, webView)
                && !ReferenceEquals(webPage2, webPage)
                && state2.SessionId != sessionBefore
                && webView2.HasLiveAdapter,
                $"关闭重开后得到新 WebView 实例，新会话={state2.SessionId}，旧会话={sessionBefore}。");
            Check("webview-reopen-default-collapsed",
                !webView2.DiagnosticsOpen && webView2.HasLiveAdapter,
                $"关闭重开的新 Web 标签默认收起诊断={!webView2.DiagnosticsOpen}，适配器可用={webView2.HasLiveAdapter}。");

            var webClosed2 = await _window.TryCloseForTestAsync(webPage2!, CloseDecision.Discard);
            var adapterDestroyed2 = await WaitUntilAsync(
                () => webView2.AdapterDestroyedCount > 0,
                TimeSpan.FromSeconds(8));
            Check("webview-second-close-releases",
                webClosed2 && adapterDestroyed2,
                "第二个 WebView 标签关闭后同样触发 AdapterDestroyed。");

            var listPage = _window.FindPage(PageKey.VisitList);
            if (listPage is not null)
            {
                await _window.TryCloseForTestAsync(listPage, CloseDecision.Discard);
                await PumpAsync(150);
            }

            Check("single-window-after-all",
                desktop is { Windows.Count: 1 },
                $"全部页面操作后顶层窗口数：{desktop?.Windows.Count.ToString() ?? "unknown"}");

            var diagnostics = _window.Diagnostics;
            Check("page-lifecycle-counts",
                diagnostics.PageCreated - diagnostics.PageReleased == 1,
                $"PageCreated={diagnostics.PageCreated}, PageReleased={diagnostics.PageReleased}, 仅首页保持打开。");
            Check("webadapter-lifecycle-counts",
                diagnostics.WebAdapterCreated >= 2 && diagnostics.WebAdapterDestroyed >= 2,
                $"WebAdapterCreated={diagnostics.WebAdapterCreated}, WebAdapterDestroyed={diagnostics.WebAdapterDestroyed}。");

            _notes.Add($"Web 初始会话：{initialState.SessionId}");
            _notes.Add($"Web 最终会话：{sessionBefore}");
            _notes.Add($"NativeWebView 适配器：{webView.AdapterDescription}");
            passed = _checks.All(check => check.Passed) && _checks.Count >= 15;
        }
        catch (Exception ex)
        {
            Check("self-test-execution", false, ex.GetBaseException().ToString());
            _notes.Add("自测在异常处终止，后续检查未执行。");
        }
        finally
        {
            stopwatch.Stop();
        }

        var result = new
        {
            schemaVersion = 1,
            status = passed ? "passed" : "failed",
            startedAt = startedAt.ToString("O"),
            finishedAt = DateTimeOffset.Now.ToString("O"),
            durationMs = stopwatch.ElapsedMilliseconds,
            environment = new
            {
                os = RuntimeInformation.OSDescription,
                framework = RuntimeInformation.FrameworkDescription,
                processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                machineName = Environment.MachineName,
                appBase = AppContext.BaseDirectory,
                workingDirectory = Environment.CurrentDirectory,
                windowClientAtStart = $"{initialClientSize.Width:0}x{initialClientSize.Height:0}",
                renderScaling = _window.RenderScaling,
                screenBounds = primaryScreen?.Bounds.ToString(),
                screenWorkingArea = primaryScreen?.WorkingArea.ToString(),
                screenScaling = primaryScreen?.Scaling,
            },
            summary = new
            {
                total = _checks.Count,
                passed = _checks.Count(check => check.Passed),
                failed = _checks.Count(check => !check.Passed),
            },
            windowCount = desktop?.Windows.Count,
            diagnostics = new
            {
                pageCreated = _window.Diagnostics.PageCreated,
                pageReleased = _window.Diagnostics.PageReleased,
                webAdapterCreated = _window.Diagnostics.WebAdapterCreated,
                webAdapterDestroyed = _window.Diagnostics.WebAdapterDestroyed,
                events = _window.Diagnostics.Events,
            },
            checks = _checks,
            notes = _notes,
        };

        try
        {
            var directory = Path.GetDirectoryName(_outputPath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_outputPath, json, new UTF8Encoding(false));
            var logPath = Path.ChangeExtension(_outputPath, ".log");
            await File.WriteAllTextAsync(logPath, BuildLog(result), new UTF8Encoding(false));
            Console.WriteLine(json);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"无法写入自测结果：{ex}");
        }

        await Task.Delay(150);
        desktop?.Shutdown(passed ? 0 : 1);
    }

    private void Check(string name, bool condition, string detail)
    {
        _checks.Add(new CheckResult(name, condition ? "PASS" : "FAIL", condition, detail));
    }

    private static bool IsInsideWindow(Control control, Window window)
    {
        if (!control.IsEffectivelyVisible)
            return false;

        var origin = control.TranslatePoint(new Point(0, 0), window);
        if (origin is not { } point)
            return false;

        const double tolerance = 1;
        return point.X >= -tolerance
               && point.Y >= -tolerance
               && point.X + control.Bounds.Width <= window.ClientSize.Width + tolerance
               && point.Y + control.Bounds.Height <= window.ClientSize.Height + tolerance;
    }

    private static bool IsHeaderFullyInside(DataGrid grid, string headerText)
    {
        var header = grid
            .GetVisualDescendants()
            .OfType<DataGridColumnHeader>()
            .FirstOrDefault(candidate =>
                string.Equals(candidate.Content?.ToString(), headerText, StringComparison.Ordinal));
        if (header is null || header.Bounds.Width <= 0)
            return false;

        var origin = header.TranslatePoint(new Point(0, 0), grid);
        if (origin is not { } point)
            return false;

        return point.X >= -1
               && point.Y >= -1
               && point.X + header.Bounds.Width <= grid.Bounds.Width + 1;
    }

    private async Task PumpAsync(int milliseconds) => await Task.Delay(milliseconds);

    private static async Task<bool> WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < timeout)
        {
            if (condition())
                return true;
            await Task.Delay(50);
        }

        return condition();
    }

    private void CaptureScreenshot(string name)
    {
        try
        {
            var client = _window.ClientSize;
            if (client.Width < 1 || client.Height < 1)
                return;

            var pixelSize = new PixelSize(
                (int)Math.Ceiling(client.Width),
                (int)Math.Ceiling(client.Height));
            using var bitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96));
            bitmap.Render(_window);

            var directory = Path.Combine(
                Path.GetDirectoryName(_outputPath) ?? AppContext.BaseDirectory,
                "screenshots");
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, name + ".png");
#pragma warning disable CS0618
            bitmap.Save(path);
#pragma warning restore CS0618
            _notes.Add($"截图：{path}");
        }
        catch (Exception ex)
        {
            _notes.Add($"截图失败 {name}: {ex.Message}");
        }
    }

    private string BuildLog(object result)
    {
        var builder = new StringBuilder();
        builder.AppendLine("HIS.Portal M1 self-test");
        builder.AppendLine($"time: {DateTimeOffset.Now:O}");
        builder.AppendLine($"status: {_checks.All(check => check.Passed) && _checks.Count >= 15} (checks={_checks.Count})");
        builder.AppendLine();
        foreach (var check in _checks)
            builder.AppendLine($"[{(check.Passed ? "PASS" : "FAIL")}] {check.Name}: {check.Detail}");
        builder.AppendLine();
        builder.AppendLine("diagnostics:");
        foreach (var entry in _window.Diagnostics.Events)
            builder.AppendLine($"  {entry}");
        builder.AppendLine();
        builder.AppendLine("json:");
        try
        {
            builder.AppendLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
            builder.AppendLine("<unavailable>");
        }

        return builder.ToString();
    }

    private sealed record CheckResult(string Name, string Status, bool Passed, string Detail);
}
