using LiteDB;

namespace DatabaseCore;

/// <summary>
/// 短信过滤数据库数据
/// </summary>
// ReSharper disable once InconsistentNaming
public class SMSFilterDatabaseData(ObjectId id)
{
    /// <summary>
    /// 数据库ID
    /// </summary>
    public ObjectId Id { get; set; } = id;
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// 启用状态
    /// </summary>
    public EnumSMSFilterEnabledStatus EnabledStatus { get; set; }
        
    /// <summary>
    /// 最大访问次数
    /// </summary>
    public int? MaxAccessCount { get; set; }
        
    /// <summary>
    /// 最小访问次数
    /// </summary>
    public int? MinAccessCount { get; set; }
        
    /// <summary>
    /// 最大创建时间
    /// </summary>
    public DateTime? MaxCreateTime { get; set; }
        
    /// <summary>
    /// 最小创建时间
    /// </summary>
    public DateTime? MinCreateTime { get; set; }
        
    /// <summary>
    /// 最大最后访问时间
    /// </summary>
    public DateTime? MaxLastAccessTime { get; set; }
        
    /// <summary>
    /// 最小最后访问时间
    /// </summary>
    public DateTime? MinLastAccessTime { get; set; }
        
    /// <summary>
    /// 分组列表
    /// </summary>
    public List<ObjectId>? GroupList { get; set; }
}

/// <summary>
/// 启用状态枚举
/// </summary>
// ReSharper disable once InconsistentNaming
public enum EnumSMSFilterEnabledStatus
{
    Enabled,
    Disabled,
    All
}