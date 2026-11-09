using HIS.Portal.Contracts;
using HIS.Portal.Diagnostics;
using HIS.Portal.Modules.Home;
using HIS.Portal.Modules.Outpatient;
using HIS.Portal.Modules.WebDemo;
using HIS.Portal.Navigation;

namespace HIS.Portal.Infrastructure.Demo;

/// <summary>
/// M1 composition root: wires the synthetic repository to the three business modules. The shell
/// only sees <see cref="IModuleFactory"/> and never constructs a view or view model itself.
/// </summary>
public sealed class DemoModuleFactory : IModuleFactory
{
    private readonly IReadOnlyList<IPageModule> _modules;

    public DemoModuleFactory(
        IVisitRepository repository,
        Action<PageKey> navigate,
        RuntimeDiagnostics diagnostics)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(navigate);
        ArgumentNullException.ThrowIfNull(diagnostics);

        _modules =
        [
            new HomeModule(repository, navigate, diagnostics),
            new OutpatientModule(repository, navigate, diagnostics),
            new WebDemoModule(diagnostics),
        ];
    }

    public bool CanCreate(PageKind kind) => _modules.Any(module => module.CanHandle(kind));

    public IWorkspacePage Create(PageKey key)
    {
        var module = _modules.FirstOrDefault(candidate => candidate.CanHandle(key.Kind))
                     ?? throw new ArgumentOutOfRangeException(nameof(key), key.Kind, "No module handles this page kind.");
        return module.Create(key);
    }
}
