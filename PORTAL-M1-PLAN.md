# Avalonia HIS 正式门户 M1 实施计划

交付对象： dsh-baton\
工作区： `ff48f460d884`（his-vue-nestjs）\
建议目录： `AvaClient/HIS.Portal/`\
阶段目标： 创建独立、可启动、可测试的正式门户基础壳。

本轮状态： 已完成连接器只读复核，未创建项目、修改源码、连接服务或执行构建。

## 一、源码核验与技术决策

已实际调用选中的「Codex with ChatGPT · 门户程序」连接器，成功执行 `workspace_info`、`list_directory`、`search_workspace` 和 `read_file`。

| 已读取源码                                                                                                  | 行号                     | 核实结论                                                   |
| ------------------------------------------------------------------------------------------------------ | ---------------------- | ------------------------------------------------------ |
| `PortalPrototype/PortalPrototype.App.csproj`                                                           | 1–40                   | .NET 10、Avalonia 12.1.2、WebView 12.0.1、本地 Fluent UI 引用 |
| `Views/MainWindow.axaml`                                                                               | 415–510                | 顶部 TabView、禁拖动、独立 PageHost                             |
| `Views/MainWindow.axaml.cs`                                                                            | 101–125、201–260        | PageKey 去重、已有实例激活、View 驻留                              |
| `Services/TabManager.cs`                                                                               | 1–175                  | 保存、放弃、取消关闭与资源释放                                        |
| `Services/PageRegistry.cs`                                                                             | 1–69                   | 页面创建与模块分发                                              |
| `Models/PageKey.cs`                                                                                    | 1–20                   | 业务类型与业务 ID 组成标签唯一键                                     |
| `Views/VisitListView.axaml.cs`                                                                         | 1–86                   | 已读取当前紧凑列表逻辑及交互入口                                       |
| `Views/VisitDetailView.axaml.cs`                                                                       | 1–140                  | 紧凑详情、资料折叠、原实例保留                                        |
| `Services/SelfTestRunner.cs`                                                                           | 435–505、相关搜索结果 677–685 | 窄窗口及 Web 100 次切换测试逻辑                                   |
| `DevClient/YFClient/YfClientDev/UI/MainForm.cs`                                                        | 675–780                | 旧门户菜单 ID、权限检查与组件打开                                     |
| `Components/DocFormManager.cs`                                                                         | 195–240                | 已打开业务页面的激活机制                                           |
| `backend-nestjs-mono/backend-main/src/app.controller.ts`                                               | 1–66                   | 公开测试路由                                                 |
| `backend-nestjs-mono/backend-main/src/setup.ts`                                                        | 1–115                  | API 前缀、URI 版本配置                                        |
| `backend-nestjs-mono/backend-main/src/modules/outpatient-treatment/outpatient-treatment.controller.ts` | 1–115                  | 门诊业务接口受 JWT 与权限守卫保护                                    |

另读取了后端公开路由装饰器、JWT 守卫、统一响应包装及模块注册源码。

目录核验：`AvaClient/HIS.Portal` 尚不存在；现有命名包含 `PortalPrototype`、`Avalonia-Fluent-UI`，未发现正式 HIS.Portal 命名规范。

曾尝试定位 `art-backend-nest`，返回 `FILE_NOT_FOUND`；实际 NestJS 主工程位于 `backend-nestjs-mono/backend-main`，已成功读取。

决策：采用 `HIS.Portal`，保留 `PortalPrototype` 原样作为迁移基线。

## 二、M1 工程结构

```
AvaClient/HIS.Portal/
├── HIS.Portal.slnx
├── README.md
├── NuGet.config
├── src/
│   └── HIS.Portal.App/
│       ├── HIS.Portal.App.csproj
│       ├── App.axaml
│       ├── Program.cs
│       ├── Shell/
│       │   ├── MainWindow.axaml
│       │   └── MainWindow.axaml.cs
│       ├── Navigation/
│       │   ├── TabManager.cs
│       │   ├── PageKey.cs
│       │   └── PageRegistry.cs
│       ├── Modules/
│       │   ├── Home/
│       │   ├── Outpatient/
│       │   └── WebDemo/
│       ├── Contracts/
│       │   ├── IVisitRepository.cs
│       │   └── IModuleFactory.cs
│       ├── Infrastructure/
│       │   ├── Demo/
│       │   ├── Nest/
│       │   └── Platform/
│       ├── Styles/
│       └── Assets/Web/demo.html
├── tests/
│   └── HIS.Portal.Tests/
└── artifacts/
    ├── build/
    ├── test/
    └── publish/
```

M1 仅建立两个 .NET 项目：应用与测试。暂不拆分大量类库。

`Contracts` 定义可替换接口；`Infrastructure` 实现本地模拟数据及未来服务适配边界。

## 三、原型迁移映射

| 原型现有模块                  | 正式项目目标              | M1 策略               |
| ----------------------- | ------------------- | ------------------- |
| MainWindow、PortalStyles | Shell、Styles        | 迁移并调整命名             |
| TabManager、PageKey      | Navigation          | 保留已验证语义             |
| PageRegistry            | Navigation          | 改为通过模块工厂创建页面        |
| HomeView                | Modules/Home        | 保留演示入口              |
| VisitList、VisitDetail   | Modules/Outpatient  | 保留窄窗口及草稿流程          |
| MockVisitRepository     | Infrastructure/Demo | 实现 IVisitRepository |
| WebDemoView、demo.html   | Modules/WebDemo     | 保留真实 NativeWebView  |
| SelfTestRunner          | 新项目测试设施             | 迁移核心回归用例            |

不直接引用 `PortalPrototype.App.dll`，避免正式门户持续依赖原型程序集。

只复制必要的已验证实现到新目录，原型不修改。

## 四、分步实施

### M1-1：独立工程与依赖

创建解决方案、应用和测试项目。

使用现有用户级 .NET 10 SDK，不安装新 SDK 或 NuGet 依赖。

版本沿用：

- Avalonia：12.1.2
- Avalonia.Controls.WebView：12.0.1
- CommunityToolkit.Mvvm：8.4.2
- Fluent UI：引用 `AvaClient/Avalonia-Fluent-UI/src/AvaloniaFluentUI/AvaloniaFluentUI.csproj`

禁止 AOT、Trim、SingleFile。

验收： Windows x64 Release 可启动独立空壳，不依赖旧客户端。

### M1-2：正式门户 Shell

迁移浅色 Fluent 主题、侧栏、业务标签及 PageHost。

必须保留：

- 单主窗口。
- PageKey 去重。
- 禁止标签拆出或创建独立业务窗口。
- 原生页面实例驻留。
- 标签关闭时保存、放弃、取消。
- 1024×540 紧凑布局。
- 1440×900 宽屏恢复。

导航结构与页面创建机制分离，为未来动态菜单留接口，但 M1 使用静态菜单。

验收： 重复打开同一业务页面只激活已有标签；取消关闭不丢草稿。

### M1-3：演示业务及数据边界

迁移首页、接诊列表、原生接诊详情。

定义 `IVisitRepository`，M1 仅实现 `DemoVisitRepository`。

默认使用内存中的合成患者数据。禁止将其标记为真实业务数据。

保持行内按钮、双击和 Enter 目标一致；保留搜索、筛选、选中行与滚动位置。

验收： 1024×540 侧栏展开时，关键列完整可见；详情首屏可看到患者身份、保存入口及完整主诉编辑框。

### M1-4：WebView 与模块生命周期

迁移 `NativeWebView` 本地 HTML 页面。

Windows 原生依赖集中到平台适配目录，为以后跨平台实现预留接口；本期不承诺非 Windows 可运行。

验收： 100 次标签切换不重建 WebView，DOM 输入、计数器、SessionId 不变；关闭后记录适配器释放，重新打开创建新实例。

### M1-5：NestJS 候选契约

源码确认存在一个低风险测试路由：

`backend-main/src/app.controller.ts:34–42`

- `GET /apicache`
- URI 版本 2
- `@Public()`
- 返回内存计数对象 `{a: number}`
- 不涉及患者业务数据

`setup.ts:43–63` 配置全局 API 前缀，默认 `api`。因此默认路径候选为 `GET /api/v2/apicache`，但实际部署配置及可访问性尚未验证。

统一响应包装见 `response-envelope.interceptor.ts:11–29`，成功结构由 `ApiResponse.ts:8–19` 定义。

M1 默认不连接该接口。

可选测试切片：仅在显式测试模式下，使用 .NET 内置 HTTP 能力启动回环地址模拟器，模拟这一响应结构；客户端仅允许访问经过校验的 `127.0.0.1` 回环地址。

不得访问现有 NestJS 实例，不得请求真实身份令牌。

已发现的门诊接口存在 JWT 和权限要求，M1 不接入，也不伪造认证契约。

## 五、M1 验收门禁

| 验收项     | 标准                     |
| ------- | ---------------------- |
| 工程独立    | 正式 EXE 不依赖原型或旧 DLL     |
| 标签管理    | 同一 PageKey 唯一，禁止独立业务窗口 |
| 草稿      | 保存、放弃、取消及失败保留正确        |
| 窄窗口     | 1024×540 两项 P0 全部通过    |
| 宽屏      | 1440×900 全列及双栏恢复       |
| WebView | 100 次切换状态保持，关闭释放       |
| 数据隔离    | 默认零外部业务请求              |
| 自动测试    | 全部通过，无失败项              |
| DPI     | 100% 实测；125% 单独实测并记录结果 |

125% DPI 未经实测前不得宣称完全兼容。测试时必须记录客户区实际 DIP、物理分辨率和 Windows 缩放比例。

## 六、构建、测试与发布

由 dsh-baton 在正式项目创建后串行执行：

```
$root = ".\AvaClient\HIS.Portal"
$app = "$root\src\HIS.Portal.App\HIS.Portal.App.csproj"
$tests = "$root\tests\HIS.Portal.Tests\HIS.Portal.Tests.csproj"

dotnet --list-sdks

dotnet restore $app -r win-x64
dotnet build $app -c Release -r win-x64 --no-restore

dotnet test $tests -c Release

dotnet publish $app -c Release -r win-x64 `
  --self-contained true `
  -p:PublishAot=false `
  -p:PublishTrimmed=false `
  -p:PublishSingleFile=false `
  -o "$root\artifacts\publish\win-x64"
```

写入隔离是执行前置门禁。 以上为逻辑命令，baton 必须先配置并验证 `UseArtifactsOutput`、`ArtifactsPath` 及 NuGet 缓存位置，使新项目和上游 ProjectReference 的生成文件均位于 `HIS.Portal` 内；不得直接以默认输出路径违反写入限制。

如无法保证隔离，停止构建并报告，不得写入上游目录。

## 七、回滚及后续迁移边界

M1 与原型完全隔离。出现构建或验收失败时，停止正式版本交付，保留失败日志及已有文件，继续使用原型作为参考基线；不得删除缓存、覆盖原型或修改上游源码。

M1 范围外：

- 真实登录与权限。
- 生产 NestJS、Oracle 读写。
- 旧 DLL/COM 直接加载。
- 真实患者数据。
- 正式组件发布平台。
- 跨平台运行支持。

M2 以后按业务模块逐项评审 NestJS 契约、认证和权限；兼容期通过明确的受控适配边界保留必要旧访问。不兼容的旧页面采用 Avalonia 原生或受控 Web 页面重写迁移，不得通过独立窗口绕过统一标签管理。

最终交付： 独立 `HIS.Portal` 解决方案、源码、Release 产物、构建日志、测试结果、DPI 验证报告及未完成迁移清单。M1 是可迁移的正式工程基础，不是可直接投入生产的 HIS 系统。
