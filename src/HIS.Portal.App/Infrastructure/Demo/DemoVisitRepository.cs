using HIS.Portal.Contracts;

namespace HIS.Portal.Infrastructure.Demo;

/// <summary>
/// In-memory repository with deterministic synthetic visits. No production service,
/// database, Oracle client or legacy DLL is involved. This is the only M1 implementation of
/// <see cref="IVisitRepository"/> and it is explicitly marked synthetic.
/// </summary>
public sealed class DemoVisitRepository : IVisitRepository
{
    public string DataSourceNotice => DemoMode.DataSourceNotice;

    public bool IsSynthetic => true;

    private readonly object _gate = new();
    private readonly List<VisitRecord> _records = BuildSeedData();

    public event EventHandler? Changed;

    public int Count
    {
        get
        {
            lock (_gate)
                return _records.Count;
        }
    }

    public IReadOnlyList<VisitRecord> GetAll()
    {
        lock (_gate)
            return _records.Select(record => record.Clone()).ToArray();
    }

    public VisitRecord? Find(string visitNo)
    {
        lock (_gate)
        {
            var record = _records.FirstOrDefault(
                item => string.Equals(item.VisitNo, visitNo, StringComparison.OrdinalIgnoreCase));
            return record?.Clone();
        }
    }

    public VisitRecord? FindFirst()
    {
        lock (_gate)
            return _records.FirstOrDefault()?.Clone();
    }

    public void Update(VisitRecord updated)
    {
        ArgumentNullException.ThrowIfNull(updated);
        lock (_gate)
        {
            var index = _records.FindIndex(
                item => string.Equals(item.VisitNo, updated.VisitNo, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
                throw new InvalidOperationException($"Visit '{updated.VisitNo}' does not exist.");

            _records[index] = updated.Clone();
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    private static List<VisitRecord> BuildSeedData()
    {
        string[] names =
        [
            "林芷晴", "周子墨", "陈思远", "许安然", "赵明轩", "沈若溪", "王嘉树", "李知夏",
            "吴承宇", "郑雨桐", "孙泽楷", "何静姝", "徐一鸣", "林书瑶", "高远航", "唐可心",
            "宋予安", "顾清越", "韩沐阳", "叶书宁", "蒋欣然", "程亦辰", "苏念初", "陆星河",
            "谢知微", "罗嘉禾", "方以宁", "白景行", "秦语棠", "江修远",
        ];

        string[] departments = ["心血管内科", "呼吸内科", "消化内科", "神经内科", "内分泌科"];
        string[] doctors = ["陈晓", "顾明", "周宁", "沈岚", "赵清"];
        string[] statuses = ["待接诊", "待接诊", "已接诊", "已完成"];
        string[] complaints =
        [
            "反复胸闷两周，活动后加重。",
            "咳嗽伴咽痛三天，夜间明显。",
            "上腹隐痛一周，餐后加重。",
            "头晕伴睡眠欠佳五天。",
            "口干多饮两周，血糖控制欠佳。",
        ];
        string[] diagnoses =
        [
            "冠状动脉粥样硬化性心脏病待排",
            "急性上呼吸道感染",
            "慢性胃炎急性发作",
            "紧张性头痛",
            "2型糖尿病，血糖控制不佳",
        ];

        var records = new List<VisitRecord>(30);
        for (var i = 0; i < 30; i++)
        {
            var number = i + 1;
            var hour = 8 + i / 4;
            var minute = i % 4 * 15;
            records.Add(new VisitRecord
            {
                VisitNo = $"MZ20261108-{number:000}",
                PatientName = names[i],
                Gender = i % 3 == 0 ? "男" : "女",
                Age = 22 + (i * 7) % 53,
                Department = departments[i % departments.Length],
                Doctor = doctors[i % doctors.Length],
                Status = statuses[i % statuses.Length],
                AppointmentTime = $"{hour:00}:{minute:00}",
                Priority = i is 3 or 11 or 23 ? "加急" : i % 5 == 0 ? "优先" : "普通",
                ChiefComplaint = complaints[i % complaints.Length],
                Diagnosis = i % 4 == 3 ? "待完善" : diagnoses[i % diagnoses.Length],
                Notes = i % 3 == 0 ? "需复查心电图与心肌酶谱。" : string.Empty,
                VisitType = i % 6 == 0 ? "专家门诊" : "普通门诊",
                InsuranceType = i % 4 == 0 ? "城乡居民医保" : "城镇职工医保",
                Phone = $"138****{1000 + i:0000}",
            });
        }

        return records;
    }
}
