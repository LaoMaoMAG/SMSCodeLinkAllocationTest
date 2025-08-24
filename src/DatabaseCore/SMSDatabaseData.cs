using LiteDB;

namespace DatabaseCore;

/// <summary>
/// 短信数据库数据
/// </summary>
/// <param name="content"></param>
// ReSharper disable once UnusedType.Global
// ReSharper disable once InconsistentNaming
public class SMSDatabaseData(string content)
{
    /// <summary>
    /// 数据 ID
    /// </summary>
    public ObjectId Id { get; set; } = null!;

    /// <summary>
    /// 短信 API 内容
    /// </summary>
    public string Content { get; set; } = content;
    
    /// <summary>
    /// 访问次数
    /// </summary>
    public int AccessCount { get; set; } = 0;
    
    /// <summary>
    /// 最后访问时间
    /// </summary>
    public DateTime LastAccessTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;
}