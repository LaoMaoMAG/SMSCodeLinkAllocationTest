using LiteDB;
using System;
using System.Threading;
using DatabaseCore.DatabaseBase;

namespace DatabaseCore;

/// <summary>
/// 数据库管理器
/// </summary>
// ReSharper disable once InconsistentNaming
public partial class SMSDatabase : SynchronizeLockDatabaseBase<SMSDatabaseData>
{
    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static SMSDatabase Instance => new ();

    /// <summary>
    /// 短信数据库表名
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private const string SMSTableName = "sms";
    
    /// <summary>
    /// 私有构造函数
    /// </summary>
    private SMSDatabase()
    {
        Init();
    }
    
    /// <summary>
    /// 添加短信
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public bool AddSMS(string content)
    {
        try
        {
            Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                col.EnsureIndex(x => x.Content, true); // 设置唯一索引
                col.Insert(new SMSDatabaseData(content));
            });
            return true; // 插入成功
        }
        catch (InvalidOperationException)
        {
            return false; // 插入失败
        }
    }
}