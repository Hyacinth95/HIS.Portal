using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HIS.Portal.Modules.Home;
using HIS.Portal.Modules.Outpatient;
using HIS.Portal.Modules.WebDemo;
using HIS.Portal.Shell;

namespace HIS.Portal;

public sealed class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var args = desktop.Args ?? [];
            var selfTestPath = ReadSelfTestPath(args);
            var mainWindow = new MainWindow(selfTestPath);
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static string? ReadSelfTestPath(IReadOnlyList<string> args)
    {
        for (var i = 0; i < args.Count; i++)
        {
            if (!string.Equals(args[i], "--self-test", StringComparison.OrdinalIgnoreCase))
                continue;

            if (i + 1 < args.Count && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                return args[i + 1];

            return Path.Combine(AppContext.BaseDirectory, "self-test-result.json");
        }

        return null;
    }
}
