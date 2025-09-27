using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using APIModes;
using APIModes.RequestModes;
using APIModes.ReturnModes;
using Microsoft.JSInterop;

namespace AdminPanel.Pages;

public partial class Home
{
    /// <summary>
    /// 管理员面板名称
    /// </summary>
    private string? _adminPanelName;
    
    /// <summary>
    /// 用户访问接码设置
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private bool _isEnableUserAccessSMS;
    
    /// <summary>
    /// 用户多接码设置
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private bool _isEnableUserMultipleVisitsAtOnceSMS;
    
    /// <summary>
    /// 用户访问接码密钥
    /// </summary>
    private string? _userAccessKey;
    
    /// <summary>
    /// 用户访问加密密钥
    /// </summary>
    private string? _userAccessEncryptionKey;

    /// <summary>
    /// 刷新用户访问接码设置
    /// </summary>
    private async Task OnRefreshUserAccessMSMSettingsChanged()
    {
        _isLoading = true;
        StateHasChanged();
        await RefreshUserAccessMSMSettings();
        _isLoading = false;
        StateHasChanged();
    }

    /// <summary>
    /// 上传用户访问接码设置
    /// </summary>
    private async Task OnUploadUserAccessMSMSettingsChanged()
    {
        _isLoading = true;
        StateHasChanged();
        await UploadUserAccessMSMSettings();
        _isLoading = false;
        StateHasChanged();
    }

    /// <summary>
    /// 生成用户访问接码密钥
    /// </summary>
    private void OnGenerateRandomUserAccessKeyChanged()
    {
        _userAccessKey = GenerateSecureRandomString(32);
    }
     
    /// <summary>
    /// 生成用户访问加密密钥
    /// </summary>
    private void OnGenerateRandomUserAccessEncryptionKeyChanged()
    {
        _userAccessEncryptionKey = GenerateSecureRandomString(32);
    }

    /// <summary>
    /// 复制用户访问密钥
    /// </summary>
    private async Task OnCopyUserAccessKeyChanged()
    {
        if (!string.IsNullOrEmpty(_userAccessKey))
        {
            await CopyToClipboard(_userAccessKey);
            ShowToast("用户访问密钥已复制到剪贴板！", "success");
        }
        else
        {
            ShowToast("用户访问密钥为空，无法复制！", "warning");
        }
    }

    /// <summary>
    /// 复制用户访问加密密钥
    /// </summary>
    private async Task OnCopyUserAccessEncryptionKeyChanged()
    {
        if (!string.IsNullOrEmpty(_userAccessEncryptionKey))
        {
            await CopyToClipboard(_userAccessEncryptionKey);
            ShowToast("用户访问加密密钥已复制到剪贴板！", "success");
        }
        else
        {
            ShowToast("用户访问加密密钥为空，无法复制！", "warning");
        }
    }

    /// <summary>
    /// 复制文本到剪贴板
    /// </summary>
    /// <param name="text">要复制的文本</param>
    private async Task CopyToClipboard(string text)
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
        catch (Exception ex)
        {
            ShowToast($"复制到剪贴板失败：{ex.Message}", "error");
        }
    }
    
    /// <summary>
    /// 刷新用户访问接码设置
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private async Task RefreshUserAccessMSMSettings()
    {
        try
        {
            var url = $"{Config.WebAPIServer}/AdminRequest/GetUserSMSUserSettings";
            var response = await Http.PostAsJsonAsync(url, new AdminRequestDataBase
            {
                Username = _username,
                Password = _password
            });

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadFromJsonAsync<AdminReturnUserSMSUserSettingsData>();
                if (jsonData?.Data == null)
                {
                    ShowToast($"获取用户访问接码设置失败！", "error");
                    return;
                }
                
                _isEnableUserAccessSMS = jsonData.Data.IsEnableUserAccessSMS;
                _isEnableUserMultipleVisitsAtOnceSMS = jsonData.Data.IsEnableUserMultipleVisitsAtOnceSMS;
                _userAccessKey = jsonData.Data.UserAccessKey;
                _userAccessEncryptionKey = jsonData.Data.UserAccessEncryptionKey;
                _adminPanelName = jsonData.Data.AdminPanelName;
                
                StateHasChanged();
            }
            else
                ShowToast($"获取用户访问接码设置时 HTTP 状态码错误：{response.StatusCode}！", "error");
        }
        catch (Exception e)
        {
            ShowToast($"获取用户访问接码设置时出现错误：{e}！", "error");
            throw;
        }
    }
    
    /// <summary>
    /// 上传用户访问接码设置
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private async Task UploadUserAccessMSMSettings()
    {
        try
        {
            var url = $"{Config.WebAPIServer}/AdminRequest/SetUserSMSUserSettings";
            
            var response = await Http.PostAsJsonAsync(url,  new AdminRequestUserSMSUserSettingsData
            {
                Username = _username,
                Password = _password,
                Data = new UserSMSUserSettingsData
                {
                    IsEnableUserAccessSMS = _isEnableUserAccessSMS,
                    IsEnableUserMultipleVisitsAtOnceSMS = _isEnableUserMultipleVisitsAtOnceSMS,
                    UserAccessKey = _userAccessKey,
                    UserAccessEncryptionKey = _userAccessEncryptionKey,
                    AdminPanelName = _adminPanelName
                }
            });

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadFromJsonAsync<ReturnDataBase>();
                if (jsonData is not { Success: true })
                {
                    ShowToast($"上传用户访问接码设置失败！", "error");
                }
                else
                {
                    ShowToast($"上传用户访问接码设置成功！", "success");
                }

                await Task.Delay(500);
                await RefreshUserAccessMSMSettings();
                StateHasChanged();
            }
            else
                ShowToast($"获取用户访问接码设置时 HTTP 状态码错误：{response.StatusCode}！", "error");
        }
        catch (Exception e)
        {
            ShowToast($"获取用户访问接码设置时出现错误：{e}！", "error");
            throw;
        }
    }
    
    /// <summary>
    /// 生成安全随机字符串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateSecureRandomString(int length = 16)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var data = new byte[length];
    
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(data);  // 生成加密安全的随机字节
        }

        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[data[i] % chars.Length];
        }

        return new string(result);
    }
}