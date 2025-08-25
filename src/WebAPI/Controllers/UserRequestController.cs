using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserRequestController : ControllerBase
{
    /// <summary>
    /// 获取单个短信
    /// </summary>
    /// <returns></returns>
    [HttpGet("RequestSingleSMS")]
    // ReSharper disable once InconsistentNaming
    public string? RequestSingleSMS()
    {
        return DatabaseCore.DatabaseManager.Instance.RequestSMS();
    }
    
    /// <summary>
    /// 获取多个短信
    /// </summary>
    /// <param name="count">获取数量</param>
    /// <returns></returns>
    [HttpGet("RequestMultipleSMS/{count}")]
    // ReSharper disable once InconsistentNaming
    public string? RequestMultipleSMS(string count)
    {
        var smsList = DatabaseCore.DatabaseManager.Instance.RequestSMS(int.Parse(count));
        return smsList?.Aggregate("", (current, sms) => current + (sms.Content + "\n"));
    }
}