using LiteDB;

namespace DatabaseCore.DatabaseBase;

/// <summary>
/// 同步锁数据库基础类
/// </summary>
public abstract class SynchronizeLockDatabaseBase
{
    /// <summary>
    /// 数据库文件路径
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    protected abstract string DatabaseFilePath { get; init; }
    
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected ConnectionString ConnectionString = null!;
    
    /// <summary>
    /// 互斥锁对象
    /// </summary>
    private readonly object _lock = new ();

    /// <summary>
    /// 初始化
    /// </summary>
    protected void Init()
    {
        ConnectionString = new ConnectionString(DatabaseFilePath)
        {
            Connection = ConnectionType.Direct
        };
        using var db = new LiteDatabase(ConnectionString);
    }
    
    /// <summary>
    /// 执行数据库操作（同步锁）
    /// </summary>
    /// <param name="operation"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected T Execute<T>(Func<LiteDatabase, T> operation)
    {
        using var db = new LiteDatabase(ConnectionString);
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
    protected void Execute(Action<LiteDatabase> operation)
    {
        using var db = new LiteDatabase(ConnectionString);
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
}