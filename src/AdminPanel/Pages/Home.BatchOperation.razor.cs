using System.Net.Http.Json;
using AdminPanel.Components;
using APIModes.RequestModes;
using APIModes.ReturnModes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AdminPanel.Pages;

// 批量操作
public partial class Home
{
    /// <summary>
    /// 导入数据对话框
    /// </summary>
    private Dialog _deleteTableSelectionItemDialog = null!;
    
    /// <summary>
    /// 是否表格全选
    /// </summary>
    private bool _isSelectAll;

    /// <summary>
    /// 禁用表格选中项目
    /// </summary>
    private async Task OnDisableTableSelectionItemChanged()
    {
        await SetTableSelectionItemIsEnable(false);
    }

    /// <summary>
    /// 启用表格选中项目
    /// </summary>
    private async Task OnEnableTableSelectionItemChanged()
    {
        await SetTableSelectionItemIsEnable(true);
    }

    /// <summary>
    /// 设置表格选中项目是否启用
    /// </summary>
    private async Task SetTableSelectionItemIsEnable(bool isEnable)
    {
        _isLoading = true;
        StateHasChanged();
        try
        {
            var url = $"{Config.WebAPIServer}/AdminRequest/SetSMSIsEnable?isEnable={isEnable}";
            var response = await Http.PostAsJsonAsync(url, new AdminRequestStringListData
            {
                Username = _username,
                Password = _password,
                Data = GetTableSelectionItemList()
            });

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadFromJsonAsync<ReturnDataBase>();

                await RefreshTable(1);

                _isLoading = false;
                StateHasChanged();

                if (jsonData is { Success: true })
                    ShowToast(isEnable ? "启用项目成功！" : "禁用项目成功！", "success");
                else
                    ShowToast(isEnable ? "启用项目失败！" : "禁用项目失败！", "error");
            }
            else
            {
                ShowToast(isEnable
                        ? $"启用项目时 HTTP 状态码错误： {response.StatusCode} ！"
                        : $"禁用项目时 HTTP 状态码错误： {response.StatusCode} ！",
                    "error");
            }

            // 重新刷新表格数据
            await RefreshTable(CurrentPage);
        }
        catch (Exception e)
        {
            _isLoading = false;
            StateHasChanged();
            ShowToast(isEnable
                    ? $"启用项目时出现错误： {e} ！" 
                    : $"禁用项目时出现错误： {e} ！",
                "error");
            throw;
        }
    }

    /// <summary>
    /// 删除表格选中项目
    /// </summary>
    private async Task OnDeleteTableSelectionItemChanged()
    {
        await _deleteTableSelectionItemDialog.Show();
    }
    
    /// <summary>
    /// 删除表格选中项目
    /// </summary>
    private async Task DeleteTableSelectionItem()
    {
        await _deleteTableSelectionItemDialog.Hide();
        
        _isLoading = true;
        StateHasChanged();
        
        try
        {
            var url = $"{Config.WebAPIServer}/AdminRequest/DeleteSMS";
            var response = await Http.PostAsJsonAsync(url, new AdminRequestStringListData
            {
                Username = _username,
                Password = _password,
                Data = GetTableSelectionItemList()
            });

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadFromJsonAsync<ReturnDataBase>();

                await RefreshTable(1);

                _isLoading = false;
                StateHasChanged();
                
                // 取消全选
                if (_isSelectAll)
                {
                    _isSelectAll = false;
                    ToggleSelectAll();
                }
                
                if (jsonData is { Success: true })
                    ShowToast("删除项目成功！", "success");
                else
                    ShowToast("删除项目失败！", "error");
            }
            else
            {
                ShowToast($"删除项目时 HTTP 状态码错误： {response.StatusCode} ！", "error");
            }

            // 重新刷新表格数据
            await RefreshTable(CurrentPage);
        }
        catch (Exception e)
        {
            await _deleteTableSelectionItemDialog.Hide();
            
            _isLoading = false;
            StateHasChanged();
            ShowToast($"禁用项目时出现错误：{e}！", "error");
            throw;
        }
        
        await _deleteTableSelectionItemDialog.Hide();
    }

    /// <summary>
    /// 导出表格选中项目
    /// </summary>
    private async Task OnExportTableSelectionItemChanged()
    {
        // 获取表格选中项目列表
        var text = GetTableSelectionItemList().Aggregate("", (current, item) => current + item + "\r\n");

        // 去除末尾换行符
        if (text.EndsWith("\r\n"))
            text = text.Substring(0, text.Length - 2);

        // 调用 JavaScript 函数保存文件
        await JsRuntime.InvokeVoidAsync("downloadFile", "export.txt", text);
    }


    /// <summary>
    /// 获取表格选中项目列表
    /// </summary>
    private List<string> GetTableSelectionItemList()
    {
        return (from item in _items where item.IsSelected select item.Content).ToList();
    }

    /// <summary>
    /// 全选/取消全选
    /// </summary>
    /// <param name="e"></param>
    private void HandleSelectAllChanged(ChangeEventArgs e)
    {
        if (e.Value is bool value)
        {
            _isSelectAll = value;
        }

        // 执行全选逻辑
        ToggleSelectAll();
    }

    /// <summary>
    /// 全选操作
    /// </summary>
    private void ToggleSelectAll()
    {
        foreach (var item in _items)
        {
            item.IsSelected = _isSelectAll;
        }
    }
}