using LiteDB;
using System;
using System.Threading;

namespace DatabaseCore;

/// <summary>
/// 数据库管理器
/// </summary>
// ReSharper disable once InconsistentNaming
public partial class SMSDatabase
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
    /// 数据库连接字符串
    /// </summary>
    private readonly ConnectionString _connectionString;
    
    /// <summary>
    /// 互斥锁对象
    /// </summary>
    private readonly object _lock = new ();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private SMSDatabase()
    {
        // 连接数据库
        _connectionString = new ConnectionString(DatabaseCoreConfig.LiteDbFilePath)
        {
            Connection = ConnectionType.Direct // 直接模式，适合单进程
        };
        using var db = new LiteDatabase(_connectionString);
        // 创建数据表并设置索引
        var collection = db.GetCollection<SMSDatabaseData>(SMSTableName);
        collection.EnsureIndex(x => x.Content, true); // 设置唯一索引
    }

    /// <summary>
    /// 执行数据库操作（同步锁）
    /// </summary>
    /// <param name="operation"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private T Execute<T>(Func<LiteDatabase, T> operation)
    {
        using var db = new LiteDatabase(_connectionString);
        try
        {
            lock (_lock)
            {
                return operation(db);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"数据库操作发生异常：{e}");
            throw;
        }
    }

    /// <summary>
    /// 执行数据库操作（同步锁）
    /// </summary>
    /// <param name="operation"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void Execute(Action<LiteDatabase> operation)
    {
        using var db = new LiteDatabase(_connectionString);
        try
        {
            lock (_lock)
            {
                operation(db);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"数据库操作发生异常：{e}");
            throw;
        }
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