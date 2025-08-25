using DatabaseCore;

namespace APIModes.ReturnModes;

/// <summary>
/// 返回短信列表数据
/// </summary>
// ReSharper disable once InconsistentNaming
public class AdminReturnSMSPageData : ReturnDataBase
{
    /// <summary>
    /// 短信列表
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public List<SMSDatabaseData>? SMSList { get; set; }
    
    /// <summary>
    /// 短信总数
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int? SMSTotal { get; set; }
    
    /// <summary>
    /// 总页数
    /// </summary>
    public int? TotalPages { get; set; }
    
    /// <summary>
    /// 最小编号
    /// </summary>
    public int? MiniNumber { get; set; }
    
    /// <summary>
    /// 最大编号
    /// </summary>
    public int? MaxNumber { get; set; }
}