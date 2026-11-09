using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using HIS.Portal.Contracts;
using HIS.Portal.Navigation;
using HIS.Portal.Modules;
using HIS.Portal.Modules.Home;
using HIS.Portal.Modules.Outpatient;
using HIS.Portal.Modules.WebDemo;
using HIS.Portal.Shell;

namespace HIS.Portal.Modules.Outpatient;

public partial class VisitListView : UserControl
{
    private const double CompactWidth = 780;
    private const double CompactHeight = 380;
    private const double WideWidth = 830;
    private const double WideHeight = 420;
    private const double CompactFixedWidth = 86 + 104 + 86 + 96 + 170;
    private const double CompactScrollReserve = 18;

    private bool _compact;
    private bool _layoutInitialized;
    private bool _applyingColumns;

    public VisitListView()
    {
        InitializeComponent();
        // DataGrid handles Enter itself for its own commit logic; tunnel the key first so the
        // prototype can open the selected record with the documented Enter shortcut.
        VisitGrid.AddHandler(KeyDownEvent, OnGridKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
        SizeChanged += (_, _) => UpdateLayoutFromBounds();
        Loaded += (_, _) => UpdateLayoutFromBounds();
    }

    internal DataGrid VisitDataGrid => VisitGrid;

    internal Button OpenSelectedButtonControl => OpenSelectedButton;

    internal ScrollViewer? DataScrollViewer =>
        VisitGrid.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();

    internal bool IsCompact => _compact;

    internal string ColumnLayout => string.Join(
        " | ",
        VisitGrid.Columns
            .OrderBy(column => column.DisplayIndex)
            .Select(column =>
                $"{(column.IsVisible ? "V" : "H")}:{column.Header}:{column.ActualWidth:0}:{column.DisplayIndex}"));

    internal void UpdateLayoutForTest(double width, double height) => UpdateCompactMode(width, height);

    private void UpdateLayoutFromBounds()
    {
        var width = ListGridHost.Bounds.Width > 1 ? ListGridHost.Bounds.Width : Bounds.Width;
        var height = ListGridHost.Bounds.Height > 1 ? ListGridHost.Bounds.Height : Bounds.Height;
        UpdateCompactMode(width, height);
    }

    private void UpdateCompactMode(double width, double height)
    {
        if (width <= 1 || height <= 1 || _applyingColumns)
            return;

        var compact = _compact
            ? !(width > WideWidth && height > WideHeight)
            : width < CompactWidth || height < CompactHeight;

        if (_layoutInitialized && compact == _compact)
        {
            if (compact)
                UpdateDepartmentWidth(width);
            return;
        }

        _compact = compact;
        _layoutInitialized = true;
        ApplyColumnLayout(width);
    }

    private void ApplyColumnLayout(double width)
    {
        _applyingColumns = true;
        try
        {
            var columns = VisitGrid.Columns;
            if (_compact)
            {
                // Keep the key columns in the documented compact order.
                columns[0].IsVisible = true;
                columns[2].IsVisible = true;
                columns[6].IsVisible = true;
                columns[7].IsVisible = true;
                columns[1].IsVisible = true;
                columns[4].IsVisible = true;
                columns[3].IsVisible = false;
                columns[5].IsVisible = false;
                columns[8].IsVisible = false;
                columns[9].IsVisible = false;

                columns[0].Width = new DataGridLength(86);
                columns[2].Width = new DataGridLength(104);
                columns[6].Width = new DataGridLength(86);
                columns[7].Width = new DataGridLength(96);
                columns[1].Width = new DataGridLength(170);
                UpdateDepartmentWidth(width);

                columns[0].DisplayIndex = 0;
                columns[2].DisplayIndex = 1;
                columns[6].DisplayIndex = 2;
                columns[7].DisplayIndex = 3;
                columns[1].DisplayIndex = 4;
                columns[4].DisplayIndex = 5;
                columns[3].DisplayIndex = 6;
                columns[5].DisplayIndex = 7;
                columns[8].DisplayIndex = 8;
                columns[9].DisplayIndex = 9;
                VisitGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            else
            {
                for (var i = 0; i < columns.Count; i++)
                {
                    columns[i].IsVisible = true;
                    columns[i].DisplayIndex = i;
                }

                columns[0].Width = new DataGridLength(82);
                columns[1].Width = new DataGridLength(170);
                columns[2].Width = new DataGridLength(92);
                columns[3].Width = new DataGridLength(128);
                columns[4].Width = new DataGridLength(116);
                columns[5].Width = new DataGridLength(108);
                columns[6].Width = new DataGridLength(96);
                columns[7].Width = new DataGridLength(96);
                columns[8].Width = new DataGridLength(112);
                columns[9].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                VisitGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }
        finally
        {
            _applyingColumns = false;
        }
    }

    private void UpdateDepartmentWidth(double hostWidth)
    {
        var available = VisitGrid.Bounds.Width > 1 ? VisitGrid.Bounds.Width : Math.Max(1, hostWidth - 2);
        var departmentWidth = Math.Max(
            110,
            available - CompactFixedWidth - CompactScrollReserve);
        var target = new DataGridLength(departmentWidth);
        if (Math.Abs(VisitGrid.Columns[4].Width.Value - departmentWidth) > 0.5)
            VisitGrid.Columns[4].Width = target;
    }

    private void OnRowOpenClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not VisitListViewModel viewModel
            || sender is not Button { Tag: VisitRecord visit })
        {
            return;
        }

        viewModel.SelectedVisit = visit;
        viewModel.OpenVisit(visit);
        e.Handled = true;
    }

    private void OnGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not VisitListViewModel viewModel || e.Source is not Visual source)
            return;

        // A double-click on the inline action button must be handled by the button only.
        if (source.FindAncestorOfType<Button>(includeSelf: true) is not null)
            return;

        // Only a real data row can open a detail. Header, blank space and filler rows have
        // no VisitRecord DataContext and are ignored.
        var row = source.FindAncestorOfType<DataGridRow>(includeSelf: true);
        if (row?.DataContext is not VisitRecord visit)
            return;

        viewModel.SelectedVisit = visit;
        viewModel.OpenVisit(visit);
        e.Handled = true;
    }

    private void OnGridKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || e.KeyModifiers != KeyModifiers.None
            || DataContext is not VisitListViewModel viewModel)
        {
            return;
        }

        // Let a focused inline action button handle its own Enter, so the keyboard path opens
        // the row that owns the button rather than whichever row happens to be selected.
        if (e.Source is Visual source
            && source.FindAncestorOfType<Button>(includeSelf: true) is not null)
        {
            return;
        }

        var visit = viewModel.SelectedVisit ?? VisitGrid.SelectedItem as VisitRecord;
        if (visit is null)
            return;

        viewModel.SelectedVisit = visit;
        viewModel.OpenVisit(visit);
        e.Handled = true;
    }
}
