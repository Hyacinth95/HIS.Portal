using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HIS.Portal.Modules.Outpatient;

public partial class VisitDetailView : UserControl
{
    private const double StackBreakpoint = 980;
    private const double CompactHeight = 520;
    private const double WideWidth = 1000;
    private const double WideHeight = 560;

    private bool _sidebarStacked;
    private bool _compact;
    private bool _layoutInitialized;
    private bool _basicInfoExpanded = true;

    public VisitDetailView()
    {
        InitializeComponent();
        SizeChanged += (_, _) => UpdateLayoutForSize();
        Loaded += (_, _) => UpdateLayoutForSize();
    }

    internal bool IsSidebarStacked => _sidebarStacked;

    internal bool IsCompact => _compact;

    internal bool IsBasicInfoExpanded => _basicInfoExpanded;

    internal string FixedIdentitySummary =>
        $"{FixedPatientName.Text}|{FixedVisitNo.Text}|{FixedDepartment.Text}";

    internal ScrollViewer DetailScrollViewer => DetailScroll;

    internal Button SaveButton => DetailSaveButton;

    internal double SidebarColumnWidth => DetailBodyGrid.ColumnDefinitions[1].Width.Value;

    internal Border BasicInfoCardControl => BasicInfoCard;

    internal Border CompactBasicInfoSummaryControl => CompactBasicInfoSummary;

    internal Button BasicInfoToggleButtonControl => BasicInfoToggleButton;

    internal TextBox ChiefComplaintBoxControl => ChiefComplaintBox;

    internal Border RecordCardControl => RecordCard;

    internal void UpdateLayoutForTest(double width, double height) => UpdateCompactMode(width, height);

    private void UpdateLayoutForSize()
    {
        var width = Bounds.Width;
        var height = Bounds.Height;
        if (width <= 1 || height <= 1)
            return;

        UpdateSidebar(width);
        UpdateCompactMode(width, height);
    }

    /// <summary>
    /// Keeps the read-only status column beside the record on wide screens and moves it below
    /// the editable record on narrow windows. Only layout properties change; the view model and
    /// the visual tree instances stay untouched.
    /// </summary>
    internal void UpdateLayoutForWidth(double width)
    {
        UpdateSidebar(width);
        UpdateCompactMode(width, Bounds.Height);
    }

    private void UpdateSidebar(double width)
    {
        var stack = width > 0 && width < StackBreakpoint;
        if (stack == _sidebarStacked)
            return;

        _sidebarStacked = stack;
        if (stack)
        {
            DetailBodyGrid.ColumnDefinitions[1].Width = new GridLength(0);
            Grid.SetColumn(DetailSidebar, 0);
            Grid.SetRow(DetailSidebar, 1);
            DetailSidebar.Margin = new Thickness(0, 12, 0, 0);
        }
        else
        {
            DetailBodyGrid.ColumnDefinitions[1].Width = new GridLength(310);
            Grid.SetColumn(DetailSidebar, 1);
            Grid.SetRow(DetailSidebar, 0);
            DetailSidebar.Margin = new Thickness(0);
        }
    }

    private void UpdateCompactMode(double width, double height)
    {
        if (width <= 1 || height <= 1)
            return;

        var compact = _compact
            ? !(width >= WideWidth && height >= WideHeight)
            : width < StackBreakpoint || height < CompactHeight;

        if (_layoutInitialized && compact == _compact)
            return;

        _compact = compact;
        _layoutInitialized = true;
        // Entering compact starts with the read-only card collapsed; returning to a wide
        // layout restores the full card. A manual toggle inside compact mode is preserved
        // until the compact layout is entered again.
        _basicInfoExpanded = !compact;
        ApplyCompactLayout();
    }

    private void ApplyCompactLayout()
    {
        CompactBasicInfoSummary.IsVisible = _compact;
        BasicInfoCard.IsVisible = !_compact || _basicInfoExpanded;
        BasicInfoToggleButton.Content = _basicInfoExpanded ? "收起资料" : "展开资料";
        DetailBodyGrid.Margin = _compact
            ? new Thickness(22, 8, 22, 10)
            : new Thickness(22, 14, 22, 20);
        DetailMainColumn.Spacing = _compact ? 8 : 12;
        BasicInfoCard.Padding = _compact ? new Thickness(12) : new Thickness(16);
        RecordCard.Padding = _compact ? new Thickness(12) : new Thickness(16);
        RecordHeaderHint.IsVisible = !_compact;
    }

    private void OnToggleBasicInfo(object? sender, RoutedEventArgs e)
    {
        if (!_compact)
            return;

        _basicInfoExpanded = !_basicInfoExpanded;
        BasicInfoCard.IsVisible = _basicInfoExpanded;
        BasicInfoToggleButton.Content = _basicInfoExpanded ? "收起资料" : "展开资料";
        e.Handled = true;
    }
}
