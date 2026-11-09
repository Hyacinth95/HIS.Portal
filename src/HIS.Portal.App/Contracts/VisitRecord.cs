namespace HIS.Portal.Contracts;

public sealed class VisitRecord
{
    public required string VisitNo { get; init; }

    public required string PatientName { get; init; }

    public required string Gender { get; init; }

    public required int Age { get; init; }

    public required string Department { get; init; }

    public required string Doctor { get; init; }

    public required string Status { get; set; }

    public required string AppointmentTime { get; init; }

    public required string Priority { get; set; }

    public required string ChiefComplaint { get; set; }

    public required string Diagnosis { get; set; }

    public required string Notes { get; set; }

    public string VisitType { get; init; } = "普通门诊";

    public string InsuranceType { get; init; } = "城镇职工医保";

    public string Phone { get; init; } = "138****0000";

    public string Initials => string.IsNullOrWhiteSpace(PatientName) ? "患" : PatientName[..1];

    public string GenderAge => $"{Gender} · {Age} 岁";

    public string OpenActionLabel => Status == "待接诊" ? "接诊" : "查看";

    public VisitRecord Clone() => new()
    {
        VisitNo = VisitNo,
        PatientName = PatientName,
        Gender = Gender,
        Age = Age,
        Department = Department,
        Doctor = Doctor,
        Status = Status,
        AppointmentTime = AppointmentTime,
        Priority = Priority,
        ChiefComplaint = ChiefComplaint,
        Diagnosis = Diagnosis,
        Notes = Notes,
        VisitType = VisitType,
        InsuranceType = InsuranceType,
        Phone = Phone,
    };
}
