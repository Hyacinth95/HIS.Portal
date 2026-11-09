using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HIS.Portal.Contracts;
using HIS.Portal.Navigation;
using HIS.Portal.Diagnostics;
using HIS.Portal.Infrastructure.Demo;

namespace HIS.Portal.Modules.Outpatient;

public sealed partial class VisitDetailViewModel : PageViewModelBase
{
    private readonly IVisitRepository _repository;
    private VisitRecord _baseline;

    public VisitDetailViewModel(VisitRecord record, IVisitRepository repository)
    {
        ArgumentNullException.ThrowIfNull(record);
        _repository = repository;
        _baseline = record.Clone();

        VisitNo = record.VisitNo;
        PatientName = record.PatientName;
        Gender = record.Gender;
        Age = record.Age;
        Department = record.Department;
        Doctor = record.Doctor;
        AppointmentTime = record.AppointmentTime;
        VisitType = record.VisitType;
        InsuranceType = record.InsuranceType;
        Phone = record.Phone;

        _chiefComplaint = record.ChiefComplaint;
        _diagnosis = record.Diagnosis;
        _notes = record.Notes;
        _status = record.Status;
        _priority = record.Priority;
        _savedAtText = "尚未修改";

        StatusOptions = ["待接诊", "已接诊", "已完成"];
        PriorityOptions = ["普通", "优先", "加急"];
        SaveCommand = new AsyncRelayCommand(SaveFromCommandAsync, () => CanSave);
        DiscardCommand = new RelayCommand(DiscardChanges, () => IsDirty);
        CloseCommand = new RelayCommand(RequestClose);
        PropertyChanged += OnEditorPropertyChanged;
    }

    public override string Title => $"接诊详情 · {VisitNo}";

    public string VisitNo { get; }

    public string PatientName { get; }

    public string Gender { get; }

    public int Age { get; }

    public string Department { get; }

    public string Doctor { get; }

    public string AppointmentTime { get; }

    public string VisitType { get; }

    public string InsuranceType { get; }

    public string Phone { get; }

    public string GenderAge => $"{Gender} · {Age} 岁";

    public string Initials => string.IsNullOrWhiteSpace(PatientName) ? "患" : PatientName[..1];

    public IReadOnlyList<string> StatusOptions { get; }

    public IReadOnlyList<string> PriorityOptions { get; }

    public IAsyncRelayCommand SaveCommand { get; }

    public IRelayCommand DiscardCommand { get; }

    public IRelayCommand CloseCommand { get; }

    public bool CanSave => IsDirty && !IsSaving;

    private bool _isDirty;

    public override bool IsDirty => _isDirty;

    [ObservableProperty]
    private string _chiefComplaint;

    [ObservableProperty]
    private string _diagnosis;

    [ObservableProperty]
    private string _notes;

    [ObservableProperty]
    private string _status;

    [ObservableProperty]
    private string _priority;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private string _savedAtText;

    [ObservableProperty]
    private string _saveState = "草稿尚未修改";

    public override async Task<bool> SaveAsync()
    {
        if (IsSaving)
            return false;

        IsSaving = true;
        SaveState = "正在保存到内存仓储…";
        try
        {
            await Task.Delay(80);
            var updated = _baseline.Clone();
            updated.ChiefComplaint = ChiefComplaint ?? string.Empty;
            updated.Diagnosis = Diagnosis ?? string.Empty;
            updated.Notes = Notes ?? string.Empty;
            updated.Status = Status ?? string.Empty;
            updated.Priority = Priority ?? string.Empty;
            _repository.Update(updated);

            _baseline = updated.Clone();
            SetIsDirty(false);
            SavedAtText = $"已保存 · {DateTime.Now:HH:mm:ss}";
            SaveState = "已写入内存仓储（演示数据）";
            return true;
        }
        catch (Exception ex)
        {
            SaveState = $"保存失败：{ex.Message}";
            return false;
        }
        finally
        {
            IsSaving = false;
            NotifyCommandState();
        }
    }

    public override void DiscardChanges()
    {
        _suppressDirtyCheck = true;
        try
        {
            ChiefComplaint = _baseline.ChiefComplaint;
            Diagnosis = _baseline.Diagnosis;
            Notes = _baseline.Notes;
            Status = _baseline.Status;
            Priority = _baseline.Priority;
            SetIsDirty(false);
            SavedAtText = "已放弃未保存更改";
            SaveState = "内容已恢复到上次保存状态";
        }
        finally
        {
            _suppressDirtyCheck = false;
        }

        NotifyCommandState();
    }

    public override void Release()
    {
        PropertyChanged -= OnEditorPropertyChanged;
    }

    private bool _suppressDirtyCheck;

    private void OnEditorPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is nameof(ChiefComplaint) or nameof(Diagnosis) or nameof(Notes)
            or nameof(Status) or nameof(Priority))
        {
            UpdateDirty();
        }

        if (args.PropertyName is nameof(IsDirty) or nameof(IsSaving))
            NotifyCommandState();
    }

    private void UpdateDirty()
    {
        if (_suppressDirtyCheck)
            return;

        var isDirty =
            !string.Equals(ChiefComplaint ?? string.Empty, _baseline.ChiefComplaint, StringComparison.Ordinal)
            || !string.Equals(Diagnosis ?? string.Empty, _baseline.Diagnosis, StringComparison.Ordinal)
            || !string.Equals(Notes ?? string.Empty, _baseline.Notes, StringComparison.Ordinal)
            || !string.Equals(Status ?? string.Empty, _baseline.Status, StringComparison.Ordinal)
            || !string.Equals(Priority ?? string.Empty, _baseline.Priority, StringComparison.Ordinal);

        SetIsDirty(isDirty);

        if (IsDirty)
        {
            SavedAtText = "有未保存更改";
            SaveState = "修改仅保留在当前标签页，切换标签不会丢失";
        }
        else if (!IsSaving)
        {
            SaveState = "内容与内存仓储一致";
        }
    }

    private void SetIsDirty(bool value)
    {
        if (_isDirty == value)
            return;

        _isDirty = value;
        OnPropertyChanged(nameof(IsDirty));
    }

    private async Task SaveFromCommandAsync()
    {
        await SaveAsync();
    }

    private void NotifyCommandState()
    {
        SaveCommand?.NotifyCanExecuteChanged();
        DiscardCommand?.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(CanSave));
    }
}
