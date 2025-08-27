using System.Net.Http.Json;
using APIModes.RequestModes;
using APIModes.ReturnModes;
using Microsoft.JSInterop;

namespace AdminPanel.Pages;

// 导入和导出
public partial class Home
{
    /// <summary>
    /// 导入数据
    /// </summary>
    private async Task OnImportDataChanged()
    {
        await _importDataDialog.Show();
    }

    /// <summary>
    /// 导入数据
    /// </summary>
    private async Task ImportData()
    {
        var fileContent = "";

        try
        {
            var content = await JsRuntime.InvokeAsync<string>("readFile");
            fileContent = content;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            ShowToast($"上传文件时出现错误：{ex.Message}", "error");
        }

        var lines = fileContent
            .Split(["\r\n", "\n", "\r"], StringSplitOptions.None) // 兼容不同平台的换行符
            .Where(line => !string.IsNullOrWhiteSpace(line)) // 去除空白行（包括空格、制表符等）
            .ToList();
        
        _isLoading = true;
        StateHasChanged();

        try
        {
            var url = $"{Config.WebAPIServer}/AdminRequest/AddMultipleSMS";
            var response = await Http.PostAsJsonAsync(url, new AdminRequestStringListData
            {
                Username = _username,
                Password = _password,
                Data = lines
            });

            await RefreshTable(CurrentPage);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadFromJsonAsync<ReturnDataBase>();

                await RefreshTable(1);

                _isLoading = false;
                StateHasChanged();

                if (jsonData is { Success: true })
                {
                    ShowToast("上传数据成功！", "success");
                }
                else
                {
                    ShowToast("上传数据失败！", "error");
                }
            }
            else
            {
                ShowToast($"上传数据失败时 HTTP 状态码错误： {response.StatusCode} ！", "error");
            }
        }
        catch (Exception e)
        {
            _isLoading = false;
            StateHasChanged();

            ShowToast($"向服务器上传数据时出现错误：{e.Message}", "error");
            throw;
        }

        await _importDataDialog.Hide();
    }
    
    /// <summary>
    /// 删除所有数据
    /// </summary>
    private async Task OnDeleteAllDataChanged()
    {
        await _deleteAllDataDialog.Show();
    }

    /// <summary>
    /// 删除所有数据
    /// </summary>
    private async Task DeleteAllData()
    {
        _isLoading = true;
        StateHasChanged();
        try
        {
            var url = $"{Config.WebAPIServer}/AdminRequest/DeleteAllSMS";
            var response = await Http.PostAsJsonAsync(url, new AdminRequestDataBase
            {
                Username = _username,
                Password = _password
            });

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadFromJsonAsync<ReturnDataBase>();

                await RefreshTable(1);

                _isLoading = false;
                StateHasChanged();

                if (jsonData is { Success: true })
                {
                    ShowToast("所有数据删除成功！", "success");
                }
                else
                {
                    ShowToast("删除所有数据失败！", "error");
                }
            }
            else
            {
                ShowToast($"删除所有数据时 HTTP 状态码错误： {response.StatusCode} ！", "error");
            }
        }
        catch (Exception e)
        {
            _isLoading = false;
            StateHasChanged();
            ShowToast($"删除所有数据时出现错误：{e}！", "error");
            throw;
        }
        await _deleteAllDataDialog.Hide();
    }

}