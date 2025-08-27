namespace AdminPanel.Pages;

public partial class Home
{
    /// <summary>
    /// 是否正在加载
    /// </summary>
    private bool _isLoading;

    /// <summary>
    /// 是否显示提示
    /// </summary>
    private bool _showToast;
    
    /// <summary>
    /// 提示信息
    /// </summary>
    private string _toastMessage = "";
    
    /// <summary>
    /// 提示类型
    /// </summary>
    private string _toastType = "info";
    
    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    private void ShowToast(string message, string type)
    {
        _toastMessage = message;
        _toastType = type;
        _showToast = true;
        StateHasChanged();

        // 3秒后自动关闭
        Task.Delay(3000).ContinueWith(_ =>
        {
            _showToast = false;
            InvokeAsync(StateHasChanged);
        });
    }
    
    /// <summary>
    /// 关闭提示
    /// </summary>
    private void CloseToast()
    {
        _showToast = false;
        StateHasChanged();
    }
}