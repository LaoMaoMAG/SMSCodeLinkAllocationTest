namespace APIModes.ReturnModes;

/// <summary>
/// 获取统计数据
/// </summary>
public class AdminReturnStatisticalData : ReturnDataBase
{
    /// <summary>
    /// 用户访问次数
    /// </summary>
    public int UserApiRequestsNumber { get; set; }
    
    /// <summary>
    /// 获取短信总数
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int SMSTotal { get; set; }
    
    /// <summary>
    /// 用户访问成功次数
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int UserApiRequestsSuccessfulSMSTotal { get; set; }
    
    /// <summary>
    /// 禁用的短信数量
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int DisableSMSQuantity { get; set; }
}