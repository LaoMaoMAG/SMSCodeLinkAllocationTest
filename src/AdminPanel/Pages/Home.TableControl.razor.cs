using System.Net.Http.Json;
using AdminPanel.Components;
using APIModes.RequestModes;
using APIModes.ReturnModes;
using DatabaseCore;
using Microsoft.AspNetCore.Components;

namespace AdminPanel.Pages;

// 表格控制
public partial class Home
{
    /// <summary>
    /// 导入数据对话框
    /// </summary>
    private Dialog _importDataDialog = null!;
    
    /// <summary>
    /// 删除所有数据对话框
    /// </summary>
    private Dialog _deleteAllDataDialog = null!;
    
    /// <summary>
    /// 最小号码
    /// </summary>
    private int _miniNumber = 0;

    /// <summary>
    /// 最大号码
    /// </summary>
    private int _maxNumber = 0;
    
    /// <summary>
    /// 表格每页显示数量
    /// </summary>
    private int TablePageSize { get; set; } = 20;

   
    // 示例：全选/取消全选
    private bool _selectAll;

    /// <summary>
    /// 全选/取消全选
    /// </summary>
    /// <param name="e"></param>
    private async Task HandleSelectAllChanged(ChangeEventArgs e)
    {
        if (e.Value is bool value)
        {
            _selectAll = value;
        }
        // 执行全选逻辑
        ToggleSelectAll();
    }

    /// <summary>
    /// 批量操作
    /// </summary>
    private void ToggleSelectAll()
    {
        foreach (var item in items)
        {
            item.IsSelected = _selectAll;
        }
    }

    /// <summary>
    /// 列表项数据
    /// </summary>
    /// <param name="content"></param>
    public class ItemData(string content) : SMSDatabaseData(content)
    {
        public bool IsSelected { get; set; } = false;
    }
    
     /// <summary>
    /// 表格每页显示数量选择对话框
    /// </summary>
    private async Task OnSelectionTablePageSizeChanged()
    {
        await GoToPage(1);
    }

    /// <summary>
    /// 总页数
    /// </summary>
    [Parameter] public int TotalPages { get; set; }
    
    /// <summary>
    /// 当前页面
    /// </summary>
    [Parameter] public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// 当前页面改变时触发
    /// </summary>
    [Parameter] public EventCallback<int> CurrentPageChanged { get; set; }
    
    /// <summary>
    /// 页面改变时触发
    /// </summary>
    [Parameter] public EventCallback OnPageChanged { get; set; }

    /// <summary>
    /// 显示当前页前后各几个页码
    /// </summary>
    private int _siblingCount = 2;
    
    /// <summary>
    /// 两端始终显示的页码数（如第1页和最后1页）
    /// </summary>
    private int _boundaryCount = 2;

    private record PaginationItem(int Page, bool IsEllipsis);

    private IEnumerable<PaginationItem> GetPaginationItems()
    {
        var pages = new List<PaginationItem>();

        int start = Math.Max(1, CurrentPage - _siblingCount);
        int end = Math.Min(TotalPages, CurrentPage + _siblingCount);

        // 添加第一页
        if (start > 1)
        {
            pages.Add(new PaginationItem(1, false));
            if (start > 2)
                pages.Add(new PaginationItem(-1, true)); // 用 -1 表示省略号，IsEllipsis=true
        }

        // 添加中间页码
        for (int i = start; i <= end; i++)
        {
            pages.Add(new PaginationItem(i, false));
        }

        // 添加最后一页
        if (end < TotalPages)
        {
            if (end < TotalPages - 1)
                pages.Add(new PaginationItem(-1, true));
            pages.Add(new PaginationItem(TotalPages, false));
        }

        return pages;
    }

    /// <summary>
    /// 跳转到指定页
    /// </summary>
    private async Task GoToPage(int page)
    {
        _isLoading = true;
        StateHasChanged();

        if (CurrentPage != page)
        {
            if (page < 1 || page > TotalPages || page == CurrentPage) return;

            CurrentPage = page;

            if (CurrentPageChanged.HasDelegate)
                await CurrentPageChanged.InvokeAsync(page);

            if (OnPageChanged.HasDelegate)
                await OnPageChanged.InvokeAsync();
        }

        await RefreshTable(page);

        _isLoading = false;
        StateHasChanged();
    }

    /// <summary>
    /// 刷新表格
    /// </summary>
    /// <param name="pageIndex"></param>
    private async Task RefreshTable(int pageIndex)
    {
        // 刷新统计数据
        await RefreshStatisticalData();

        try
        {
            var url = $"{Config.WebAPIServer}/AdminRequest/GetSMSPageData?pageIndex={pageIndex}&pageSize={TablePageSize}";
            var response = await Http.PostAsJsonAsync(url, new AdminRequestDataBase
            {
                Username = _username,
                Password = _password
            });

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadFromJsonAsync<AdminReturnSMSPageData>();

                _smsTotal = (int)jsonData!.SMSTotal!;
                _miniNumber = (int)jsonData.MiniNumber!;
                _maxNumber = (int)jsonData.MaxNumber!;
                TotalPages = (int)jsonData.TotalPages!;

                items.Clear();

                if (jsonData.SMSList == null) return;

                foreach (var data in jsonData.SMSList)
                {
                    items.Add(new ItemData(data.Content)
                    {
                        Id = data.Id,
                        IsEnable = data.IsEnable,
                        AccessCount = data.AccessCount,
                        LastAccessTime = data.LastAccessTime,
                        CreateTime = data.CreateTime,
                    });
                }

                StateHasChanged();
            }
            else
                ShowToast($"获取分页数据时 HTTP 状态码错误：{response.StatusCode}！", "error");
        }
        catch (Exception e)
        {
            ShowToast($"获取分页数据时出现错误：{e}！", "error");
            throw;
        }
    }

    /// <summary>
    /// 数据列表
    /// </summary>
    private List<ItemData> items = new();

}