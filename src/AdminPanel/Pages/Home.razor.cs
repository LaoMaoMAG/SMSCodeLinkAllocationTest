using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AdminPanel.Pages;

public partial class Home : ComponentBase, IDisposable
{
    /// <summary>
    /// JvavScript 运行时
    /// </summary>
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    
    /// <summary>
    /// HTTP 请求
    /// </summary>
    [Inject] public HttpClient Http { get; set; } = null!;
    
    /// <summary>
    /// 登录用户名
    /// </summary>
    private string _username = "";

    /// <summary>
    /// 登录密码
    /// </summary>
    private string _password = "";

    /// <summary>
    /// 初始化方法
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        StateHasChanged();

        // 必须比其他初始化方提前
        var cookieService = new CookieService(JsRuntime);
        _username = await cookieService.GetCookie("username");
        _password = await cookieService.GetCookie("password");

        // 刷新服务器启动时间
        await RefreshServerStartTime();

        // 刷新表格
        await RefreshTable(1);

        _isLoading = false;
        StateHasChanged();

        // 启动定时器，每秒更新一次运行时长
        _ = new Timer(_ =>
        {
            UpdateUptime();
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        // 10秒刷新一次统计数据，先等待10秒再开始执行
        _ = new Timer(refresh =>
        {
            _ = RefreshStatisticalData();
        }, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
    }
    
    /// <summary>
    /// 关闭事件
    /// </summary>
    public void Dispose()
    {
        
    }
}
