# HIS 正式门户 M1

独立、可启动、可测试的 HIS 临床门户基础壳。当前版本是**离线演示 M1**，用于建立正式工程边界、复用已验收页面和桌面回归设施；**不是可直接投入生产的 HIS 系统**。

> **数据声明：** 所有患者、就诊号、诊断、排班和 Web 表单内容均为本地合成数据。M1 不连接 NestJS、Oracle、旧 DLL/COM，不访问任何现有服务或数据库，也不使用真实身份令牌。

## 0. 一分钟验收

```powershell
$dotnet = Join-Path ([Environment]::GetFolderPath('LocalApplicationData')) 'Microsoft\dotnet\dotnet.exe'
$root = 'F:\Project\.vueTest\his-vue-nestjs\AvaClient\HIS.Portal'

# 交接方可直接运行（Debug 产物已准备好）
& $dotnet test "$root\HIS.Portal.slnx" -c Debug --no-build --no-restore --nologo -m:1

# 启动正式发布版
& "$root\artifacts\publish\win-x64\HIS.Portal.App.exe"
```

已实测结果：

| 项目 | 结果 |
| --- | --- |
| 正式项目的自动测试 | 21/21 通过（Debug 与 Release 均实测） |
| 发布版真实桌面自测 | 47/47 检查通过，退出码 0 |
| 自包含发布 | `artifacts/publish/win-x64`，含 `coreclr.dll` / `hostfxr.dll` / 正式 EXE |
| 当前显示环境 | 2560×1440，Windows 缩放 100%，`renderScaling=1`、`screenScaling=1` |
| 125% DPI | 本机当前不可用；未改变系统 DPI，**未实测**，不宣称兼容 |

## 1. 交付物

| 路径 | 说明 |
| --- | --- |
| `HIS.Portal.slnx` | 正式解决方案，只包含应用与测试两个门户项目 |
| `src/HIS.Portal.App/` | 正式门户应用 |
| `tests/HIS.Portal.Tests/` | 自动测试 |
| `third_party/AvaloniaFluentUI/` | 可复现的 Fluent UI 构建副本（依赖项目，不属于解决方案的两个门户项目） |
| `artifacts/publish/win-x64/HIS.Portal.App.exe` | 可运行的 Windows x64 自包含 EXE |
| `artifacts/verification/published/self-test.json` | 发布版真实桌面回归结果（47 项） |
| `artifacts/verification/published/screenshots/` | 发布版实际窗口截图 |
| `artifacts/verification/DPI-REPORT.md` | DPI / 分辨率实测记录 |
| `artifacts/verification/VERIFICATION.md` | 门禁逐项证据索引 |
| `PORTAL-M1-PLAN.md` | 原计划原文，未改动 |

## 2. 本机环境与依赖来源

实测环境（2026-11-09，Asia/Shanghai）：

- Windows 10.0.22621，x64。
- 用户级 .NET SDK **10.0.403**：`%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe`。
- .NET Runtime **10.0.14**。
- Microsoft Edge WebView2 Runtime **157.0.4341.42**（WebView2 / Blink）。
- 屏幕 2560×1440，工作区 2560×1440，Windows 缩放 **100%**。
- 路径中的 `dotnet` 默认解析到 .NET 9；本文所有命令显式使用上面的 `$dotnet`。

直接依赖版本：

- Avalonia `12.1.2`
- Avalonia.Desktop `12.1.2`
- Avalonia.Controls.DataGrid `12.1.2`
- Avalonia.Controls.WebView `12.0.1`
- CommunityToolkit.Mvvm `8.4.2`
- Microsoft.NET.Test.Sdk `18.0.1`
- xunit `2.9.3`
- xunit.runner.visualstudio `3.1.5`

### 2.1 NuGet：只读复用已装包

本机全局包缓存在 `F:\nuget\.nuget\packages`，M1 所需版本均已存在。`NuGet.config`：

- 清空远程源，只保留一个**有意为空**的本地源 `.nuget-empty-feed`；
- 将 `globalPackagesFolder` 指向 `F:\nuget\.nuget\packages`；
- 关闭 NuGet 审计，避免为漏洞数据发起网络请求；
- 若缺少某个包，还原将以 NU1101 失败，而不是下载或写入共享缓存。

实际还原命令带 `--no-http-cache --disable-parallel`，两次还原均成功，未向该缓存新增或修改包文件（见第 11 节与 `ISOLATION-AUDIT.txt`）。应用代码中没有 `HttpClient`、`WebRequest` 或对 NestJS 的调用。

### 2.2 Fluent UI 构建副本（关键隔离决定）

上游 `AvaloniaFluentUI.csproj` 的 MicroCom 项把生成文件硬编码到自己的源码树：

```xml
<MicroComIdl Include="$(MSBuildThisFileDirectory)\FluentControls\Interop\WinRT\WinRT.idl"
            CSharpInteropPath="$(MSBuildThisFileDirectory)\FluentControls\Interop\WinRT\WinRT.Generated.cs" />
```

因此仅设置 `ArtifactsPath` 不能阻止构建写回上游源码。M1 采用**最小可维护的源码副本**方案：

- 来源：`AvaClient/Avalonia-Fluent-UI`，版本 `2.0.6`，commit `8cc9f01c1a16468fd11eeca179d2e2b0fc88ff24`。
- 只复制 `src/AvaloniaFluentUI/**`，不含 `.git`、`bin`、`obj`，并保留 `LICENSE`、`VERSION` 和 `src/SourceLink.props`（含 `EnsureTrailingSlash` 兼容补丁）。
- 副本的 `AvaloniaFluentUI.csproj` 删除 `MicroComIdl` 项和构建期 `MicroCom.CodeGenerator.MSBuild`，直接编译仓库中已有的 `WinRT.Generated.cs`；因此构建不会生成或覆盖任何上游/副本源码文件。
- 副本不导入 SourceLink（它不是 Git 工作树），但保留补丁文件供后续更新参考。
- 不构建、不修改真实上游 checkout。

详见 `third_party/AvaloniaFluentUI/PROVENANCE.md`。

### 2.3 构建输出隔离

`Directory.Build.props` 设置：

```xml
<ArtifactsPath>$(MSBuildThisFileDirectory)artifacts</ArtifactsPath>
<UseArtifactsOutput>true</UseArtifactsOutput>
```

应用、测试和 vendored Fluent UI 的 `bin`/`obj`/publish 中间产物都落在 `HIS.Portal/artifacts` 下（`artifacts/bin`、`artifacts/obj`）。构建临时目录重定向到 `HIS.Portal/.tmp`；其中的随机空目录是 MSBuild/编译器的临时槽，不是项目源码结构。上游 checkout、旧门户、NestJS、PortalPrototype 和 Fluent UI checkout 均未产生新的写入。

## 3. 工程结构

```text
HIS.Portal/
├─ HIS.Portal.slnx                    # 两个门户项目
├─ README.md
├─ NuGet.config                       # 空本地源 + 只读复用已装包
├─ Directory.Build.props              # ArtifactsPath / UseArtifactsOutput
├─ PORTAL-M1-PLAN.md                  # 原计划，保留
├─ src/HIS.Portal.App/
│  ├─ App.axaml / App.axaml.cs
│  ├─ Program.cs
│  ├─ app.manifest
│  ├─ Shell/                          # MainWindow、Shell VM、全局搜索
│  ├─ Navigation/                     # PageKey/PageKind、TabManager、PageDocument、
│  │                                  # PageRegistry、NavigationCatalog、WorkspacePage
│  ├─ Modules/
│  │  ├─ Home/
│  │  ├─ Outpatient/                  # 列表、详情、草稿
│  │  └─ WebDemo/                     # NativeWebView 本地 HTML
│  ├─ Contracts/                      # IVisitRepository、IPageModule、IModuleFactory、VisitRecord
│  ├─ Infrastructure/
│  │  ├─ Demo/                        # DemoVisitRepository、DemoModuleFactory、DemoMode
│  │  ├─ Platform/                    # Windows WebView 适配边界
│  │  └─ Nest/                        # 仅记录后端候选契约，不联网
│  ├─ Diagnostics/                    # RuntimeDiagnostics、SelfTestRunner
│  ├─ Styles/
│  └─ Assets/Web/demo.html
├─ tests/HIS.Portal.Tests/            # 21 个自动测试
├─ third_party/AvaloniaFluentUI/      # 来源快照 + LICENSE/VERSION/SourceLink.props
└─ artifacts/
   ├─ bin/ obj/                       # 隔离的构建输出
   ├─ build/                          # Debug/Release 构建日志
   ├─ test/                           # 自动测试日志
   ├─ publish/win-x64/                # 自包含发布
   └─ verification/                   # 自测 JSON/日志/截图/DPI 报告
```

## 4. 架构边界（M1 的实际落点）

| 边界 | 文件 | 实际调用点 |
| --- | --- | --- |
| Shell | `Shell/MainWindow.axaml.cs` | 只负责窗口、标签条、PageHost、关闭确认浮层；不直接创建 View/VM |
| Navigation | `Navigation/TabManager.cs`、`PageRegistry.cs`、`NavigationCatalog.cs` | PageKey 去重、保存/放弃/取消策略、静态导航目录；不依赖 Avalonia 控件 |
| Module | `Modules/*/*Module.cs` | 每个业务模块拥有自己的 View + ViewModel 组合 |
| Contracts | `Contracts/IVisitRepository.cs`、`IModuleFactory.cs`、`VisitRecord.cs` | VM 依赖仓储接口；`PageRegistry` 依赖模块工厂接口 |
| Infrastructure | `Infrastructure/Demo/DemoVisitRepository.cs`、`DemoModuleFactory.cs` | 唯一 M1 数据/组合实现，明确合成数据标识 |
| Platform | `Infrastructure/Platform/WebViewPlatform.cs` | WebView 原生句柄、适配器描述和释放集中在此 |
| Diagnostics | `Diagnostics/RuntimeDiagnostics.cs`、`SelfTestRunner.cs` | 页面/适配器生命周期计数；真实窗口回归 |

```text
MainWindow
  ├─ TabManager (dedupe / close policy)
  ├─ PageRegistry (new PageKey only)
  │    └─ IModuleFactory
  │         └─ DemoModuleFactory
  │              ├─ HomeModule
  │              ├─ OutpatientModule ── IVisitRepository
  │              └─ WebDemoModule
  └─ PageHost (long-lived View instances)
```

### 4.1 数据隔离

- `DemoVisitRepository` 实现 `IVisitRepository`，内存中 30 条确定性合成记录；`IsSynthetic=true`，`DataSourceNotice` 显示“全部为合成数据 · 不连接生产系统”。
- 列表/详情保存只更新进程内仓储；不访问 Oracle、NestJS、旧 DLL/COM。
- 没有任何认证桩、令牌或伪造登录。
- 后端候选 `GET /api/v2/apicache` 只在 `Infrastructure/Nest/NestApiCacheCandidate.cs` 和 README 中记录；**未实现 HTTP 客户端，未调用**。该候选会改变内存计数，且实际部署前缀/可访问性未验证。

### 4.2 WebView 生命周期

- `WebDemoView` 持有 `NativeWebView`，页面切换只改变可见性，不重建 View。
- `Infrastructure/Platform/WebViewPlatform.cs` 集中处理 `AdapterInfo`、原生句柄和 `IDisposable` 适配器；`WebDemoView.Dispose` 先移出父面板再释放适配器。
- 关闭后重新打开创建新 WebView 实例和新 JS 会话；100 次标签往返保持同一实例、DOM 文本、计数和 SessionId。

## 5. 构建、测试、发布、运行

所有命令从 `F:\Project\.vueTest\his-vue-nestjs\AvaClient\HIS.Portal` 执行。使用用户级 .NET 10 SDK，串行 `-m:1 --disable-build-servers`。

```powershell
$dotnet = Join-Path ([Environment]::GetFolderPath('LocalApplicationData')) 'Microsoft\dotnet\dotnet.exe'
$root = 'F:\Project\.vueTest\his-vue-nestjs\AvaClient\HIS.Portal'
```

### 5.1 Debug（交接默认配置）

```powershell
& $dotnet restore "$root\src\HIS.Portal.App\HIS.Portal.App.csproj" `
  -r win-x64 --configfile "$root\NuGet.config" --disable-parallel --no-http-cache

& $dotnet restore "$root\tests\HIS.Portal.Tests\HIS.Portal.Tests.csproj" `
  --configfile "$root\NuGet.config" --disable-parallel --no-http-cache

& $dotnet build "$root\HIS.Portal.slnx" -c Debug --no-restore -m:1 --disable-build-servers

# 交接独立检查使用这一条（已实测通过）
& $dotnet test "$root\HIS.Portal.slnx" -c Debug --no-build --no-restore --nologo -m:1
```

Debug 产物：

- `artifacts/bin/HIS.Portal.App/debug/HIS.Portal.App.exe`
- `artifacts/bin/HIS.Portal.Tests/debug/HIS.Portal.Tests.dll`

### 5.2 Release 与自包含发布

```powershell
& $dotnet build "$root\src\HIS.Portal.App\HIS.Portal.App.csproj" `
  -c Release -r win-x64 --no-restore -m:1 --disable-build-servers

& $dotnet publish "$root\src\HIS.Portal.App\HIS.Portal.App.csproj" `
  -c Release -r win-x64 --self-contained true --no-restore -m:1 --disable-build-servers `
  -p:PublishAot=false -p:PublishTrimmed=false -p:PublishSingleFile=false `
  -o "$root\artifacts\publish\win-x64"
```

发布产物约 218 MiB（228 个顶层文件；含 WebView2 运行缓存共 416 个文件）；含 `coreclr.dll`、`hostfxr.dll`、`hostpolicy.dll`、`HIS.Portal.App.exe` 和本地 `Assets/Web/demo.html`。AOT/Trim/SingleFile 均关闭。

### 5.3 运行与桌面回归

```powershell
# 正常启动
& "$root\artifacts\publish\win-x64\HIS.Portal.App.exe"

# 真实桌面自测：创建主窗口和真实 NativeWebView，写 JSON/日志/截图后自动退出
& "$root\artifacts\publish\win-x64\HIS.Portal.App.exe" `
  --self-test "$root\artifacts\verification\published\self-test.json"
```

自测全通过返回 0，失败返回 1。结果同时写同名 `.log`。

### 5.4 测试范围

`tests/HIS.Portal.Tests` 共 21 项，覆盖：

- PageKey 规范化与同一业务键去重；
- 不同业务 ID 并开；
- 取消关闭保留脏草稿；保存关闭写入仓储；放弃不覆盖已保存数据；保存失败不关闭；
- 不可关闭首页；
- 接诊列表筛选、选择保持/清除、行内按钮与选中项使用同一 PageKey；
- 首页待接诊队列；
- `NavigationCatalog` 三个 M1 入口、`DemoModuleFactory.CanCreate`、`PageRegistry` 对模块工厂的委托与拒绝未知类型；
- 后端候选契约常量仅作为记录（不联网）。

## 6. 验收门禁结果

| 门禁 | 结果 | 证据 |
| --- | --- | --- |
| 工程独立 | 通过 | `artifacts/publish/win-x64/HIS.Portal.App.deps.json` 无 `PortalPrototype`；EXE 自包含运行；解决方案只有 App/Tests 两个门户项目 |
| 标签管理 | 通过 | 自测 `same-business-id-deduplicated`、`different-business-id-parallel`；单测 `TabManagerTests` |
| 单窗口 / 禁止独立业务窗口 | 通过 | 自测 `single-window-before`、`single-window-after-all`、`drag-tearout-disabled` |
| 草稿保存/放弃/取消/失败 | 通过 | 单测 8 项；自测 `cancel-close-preserves-draft`、`save-close-updates-memory`、`discard-close-keeps-saved-data` |
| 1024×540 窄窗 | 通过 | 自测 `narrow-client-size-1024x540`、`narrow-list-key-columns`、`narrow-detail-first-screen-edit`、`narrow-window-critical-actions`；截图 02/03 |
| 1440×900 宽屏 | 通过 | 自测 `wide-layout-restored`；截图 01/04 |
| Web 100 次切换 | 通过 | 自测 `webview-100-switch-instance`、`webview-100-switch-dom-state` |
| Web 关闭释放/重开新实例 | 通过 | 自测 `webview-close-releases-native`、`webview-reopen-new-instance`、`webview-second-close-releases` |
| 默认零外部业务请求 | 通过 | 无 `HttpClient`；`DemoVisitRepository`；空本地 NuGet 源；Web 页面为 `file://` 本地 HTML |
| 自动测试 | 通过 | Debug 21/21；Release 21/21（见 `artifacts/test/`） |
| DPI 100% | 通过 | 自测环境 `renderScaling=1`、`screenScaling=1`、2560×1440；见 DPI 报告 |
| DPI 125% | 未实测 | 本机当前缩放为 100%，未改变系统 DPI；不得用 1024×540 代替 125% |

自测实际计数：`PageCreated=7`、`PageReleased=6`、`WebAdapterCreated=2`、`WebAdapterDestroyed=2`；WebView2 / Blink `157.0.4341.42`；关闭重开后 JS 会话变化。

## 7. 证据索引

| 证据 | 路径 |
| --- | --- |
| 发布版自测 JSON | `artifacts/verification/published/self-test.json` |
| 发布版自测日志 | `artifacts/verification/published/self-test.log` |
| 发布版截图 | `artifacts/verification/published/screenshots/` |
| Debug 自测 | `artifacts/verification/self-test-debug.json` / `.log` |
| Debug 构建日志 | `artifacts/build/build-debug-solution.log`、`build-debug-app.log`、`build-debug-tests.log` |
| Release 构建日志 | `artifacts/build/build-release-app.log` |
| 发布日志 | `artifacts/publish.log` |
| Debug/Release 测试 | `artifacts/test/test-debug-handoff.log`、`test-debug.log`、`test-release.log` |
| 还原日志 | `artifacts/restore-app.log`、`restore-tests.log` |
| DPI 报告 | `artifacts/verification/DPI-REPORT.md` |
| 环境与磁盘 | `artifacts/verification/environment.txt` |
| 实际命令记录 | `artifacts/verification/COMMANDS.md` |
| 写入隔离审计 | `artifacts/verification/ISOLATION-AUDIT.txt` |
| 门禁汇总 | `artifacts/verification/VERIFICATION.md` |

**截图说明：** Avalonia `RenderTargetBitmap` 捕获的是 Avalonia 合成层，`NativeWebView` 的原生浏览器表面不在其中，因此 `05-web-demo.png` 只显示页面外壳（状态为“WebView2 已连接”）。真实 DOM、计数、SessionId 和适配器证据在自测 JSON/日志的 `real-webview-ready`、`webview-dom-editable`、`webview-100-switch-*` 项中。

## 8. 计划与实际命令差异

| 计划 | 实际 | 原因 |
| --- | --- | --- |
| `dotnet restore $app -r win-x64` | 增加 `--configfile .\NuGet.config --disable-parallel --no-http-cache` | 本机全局 NuGet source mapping 与同级 checkout 源名冲突；改用空本地源并只读复用已装包 |
| `dotnet test $tests -c Release` | 先 Debug 构建，再按交接命令 `dotnet test .slnx -c Debug --no-build --no-restore`；另跑 Release 测试 | 满足“默认配置可直接 --no-build 检查”的交付要求 |
| 默认输出路径 | `Directory.Build.props` 的 `ArtifactsPath`/`UseArtifactsOutput` | 写入严格限制在正式目录 |
| 直接 ProjectReference 上游 Fluent UI | 引用 `third_party/AvaloniaFluentUI` 源码副本 | 上游 `MicroComIdl` 的 `CSharpInteropPath` 硬编码，直接构建会写回上游 |
| 可选回环 Nest 切片 | 跳过，仅记录候选 | 非门禁；避免任何服务访问和计数副作用 |
| 125% DPI 实测 | 未实测 | 本机当前仅 100%；未改变系统 DPI |

## 9. 后续迁移清单（M2+）

1. **认证与权限**：接入真实 JWT/权限守卫；删除任何演示账号假设；不伪造认证契约。
2. **NestJS 契约**：先验证部署前缀和 `GET /api/v2/apicache` 的可访问性/副作用；用真实 DTO、错误信封、超时和重试策略替换候选记录。
3. **Oracle/数据访问**：新增 `IVisitRepository` 的生产实现，保留 `DemoVisitRepository` 作为开发/演示实现；明确读写事务与审计。
4. **动态菜单**：用后端菜单替换 `NavigationCatalog` 静态列表，保持 PageKey 去重和单窗口策略不变。
5. **迁移旧页面**：按模块评审旧 DLL/COM 访问，优先 Avalonia 原生或受控 Web 页面；禁止用独立窗口绕过统一标签管理。
6. **跨平台**：为 `IWebViewPlatformAdapter` 增加 Linux/macOS 实现后再声明支持；M1 仅 Windows。
7. **可访问性与视觉**：补充键盘/UI Automation 回归、125%/150% DPI 实测、对比度检查。
8. **打包与发布**：签名、版本号、更新通道、崩溃上报；当前不启用 AOT/Trim/SingleFile。
9. **测试**：增加真实后端契约测试、权限负向测试、断网/超时测试、WebView 缺失分支。

## 10. 已知限制

- **125% DPI 未实测**；本机 100% 实测。不得把 1024×540 的 DIP 结果解释为 125% 结果。
- WebView 原生表面不能被 Avalonia 截图捕获；DOM/会话/适配器证据来自自测。
- M1 不验证真实 WebView2 Runtime 缺失分支（本机已安装）；代码有 12 秒超时/不可用提示。Debug 自测在一次连续运行中曾遇到 WebView2 原生适配器 12 秒超时，等待 3 秒后重试通过；发布版自测 47/47。M1 未加入自动重试，以免掩盖原生环境问题。
- Fluent UI 是 vendored 快照，升级需按 `PROVENANCE.md` 重新复制并复核项目文件差异。
- 当前磁盘剩余空间较紧（F: 约 0.9 GiB）；`artifacts/bin` 含多配置/多 RID 构建产物。交接时不要执行清理/删除；如磁盘进一步不足，先报告实际空间与失败命令。
- 未执行 `git init/commit/push`，未修改旧门户、NestJS、PortalPrototype 或 Fluent UI checkout。

## 11. 构建审计

- 上游 `AvaClient/Avalonia-Fluent-UI`：本轮无新写入（按 2026-11-09 09:30 起始时间检查）。
- `AvaClient/PortalPrototype`：本轮无新写入。
- `F:\nuget\.nuget\packages`：本轮无新写入。
- `%APPDATA%\NuGet`：本轮无新写入。
- 所有新文件均在 `AvaClient/HIS.Portal` 内。

## 12. 许可

`third_party/AvaloniaFluentUI/LICENSE` 为上游 MIT 许可原文；`VERSION` 为 `2.0.6`；来源 commit 与复制范围见 `PROVENANCE.md`。









