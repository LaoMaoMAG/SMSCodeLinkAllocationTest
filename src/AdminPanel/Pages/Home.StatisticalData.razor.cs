using System.Net.Http.Json;
using APIModes.RequestModes;
using APIModes.ReturnModes;

namespace AdminPanel.Pages;

public partial class Home
{
    /// <summary>
    /// 用户API请求数据
    /// </summary>
    private int _userApiRequestsNumber;

    /// <summary>
    /// 用户API请求成功数
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private int _userApiRequestsSuccessfulSMSTotal;

    /// <summary>
    /// 禁用的短信数量
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private int _disableSMSQuantity;
    
    /// <summary>
    /// 短信总数
    /// </summary>
    private int _smsTotal;
    
    /// <summary>
    /// 运行时长
    /// </summary>
    private TimeSpan _uptime = TimeSpan.Zero;
    
    /// <summary>
    /// 服务器启动时间
    /// </summary>
    private DateTime? _serverStartTime = DateTime.Now;

    /// <summary>
    /// 刷新统计数据
    /// </summary>
    private async Task RefreshStatisticalData()
    {
        try
        {
            var url = $"{Config.WebAPIServer}/AdminRequest/GetStatisticalData";
            var response = await Http.PostAsJsonAsync(url, new AdminRequestDataBase
            {
                Username = _username,
                Password = _password
            });

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadFromJsonAsync<AdminReturnStatisticalData>();
                if (jsonData is not { Success: true })
                {
                    ShowToast($"获取统计数据失败！", "error");
                    return;
                }

                _smsTotal = jsonData.SMSTotal;
                _userApiRequestsNumber = jsonData.UserApiRequestsNumber;
                _userApiRequestsSuccessfulSMSTotal = jsonData.UserApiRequestsSuccessfulSMSTotal;
                _disableSMSQuantity = jsonData.DisableSMSQuantity;

                StateHasChanged();
            }
            else
                ShowToast($"获取统计数据时 HTTP 状态码错误：{response.StatusCode}！", "error");
        }
        catch (Exception e)
        {
            ShowToast($"获取统计数据时出现错误：{e}！", "error");
            throw;
        }
    }
    
    /// <summary>
    /// 更新运行时长
    /// </summary>
    private void UpdateUptime()
    {
        if (_serverStartTime == null) return;
        _uptime = DateTime.Now - (DateTime)_serverStartTime;
        InvokeAsync(StateHasChanged);
    }
    
    /// <summary>
    /// 刷新服务器启动时间
    /// </summary>
    private async Task RefreshServerStartTime()
    {
        try
        {
            var url = $"{Config.WebAPIServer}/AdminRequest/GetServerStartTime";
            var response = await Http.PostAsJsonAsync(url, new AdminRequestDataBase
            {
                Username = _username,
                Password = _password
            });

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadFromJsonAsync<ReturnLongData>();
                if (jsonData?.Data == null)
                {
                    ShowToast($"获取服务器启动时间失败！", "error");
                    return;
                }

                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                _serverStartTime = epoch.AddMilliseconds((long)jsonData.Data!).ToLocalTime();
            }
            else
                ShowToast($"获取服务器启动时间时 HTTP 状态码错误：{response.StatusCode}！", "error");
        }
        catch (Exception e)
        {
            ShowToast($"获取服务器启动时间时出现错误：{e}！", "error");
            throw;
        }
    }
}