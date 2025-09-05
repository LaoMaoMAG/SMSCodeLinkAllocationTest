using LiteDB;

namespace DatabaseCore
{
    /// <summary>
    /// 数据库管理器
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class SMSDatabaseManager
    {
        /// <summary>
        /// 静态锁，用于确保单线程操作
        /// </summary>
        private static readonly object Lock = new();
        
        /// <summary>
        /// 用于跟踪当前线程是否已经持有锁，防止死锁
        /// </summary>
        private static readonly ThreadLocal<bool> IsHoldingLock = new();
        
        /// <summary>
        /// 单例实例
        /// </summary>
        private static SMSDatabaseManager? _instance;
        
        /// <summary>
        /// 数据库连接
        /// </summary>
        private readonly LiteDatabase _db;
        
        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static SMSDatabaseManager Instance => _instance ??= new SMSDatabaseManager();

        /// <summary>
        /// 短信数据库表名
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private const string SMSTableName = "sms";

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private SMSDatabaseManager()
        {
            // 连接数据库
            var connectionString = new ConnectionString(DatabaseCoreConfig.LiteDbFilePath)
            {
                Connection = ConnectionType.Direct // 直接模式，适合单进程
            };
            _db = new LiteDatabase(connectionString);
            
            // 创建数据表并设置索引
            var collection = _db.GetCollection<SMSDatabaseData>(SMSTableName);
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
            // 防止同一线程重复获取锁导致死锁
            if (IsHoldingLock.Value)
            {
                try
                {
                    return operation(_db);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("数据库操作失败", ex);
                }
            }
            
            lock (Lock)
            {
                IsHoldingLock.Value = true;
                try
                {
                    return operation(_db);
                }
                catch (Exception ex)
                {
                    // 日志记录建议在这里添加
                    throw new InvalidOperationException("数据库操作失败", ex);
                }
                finally
                {
                    IsHoldingLock.Value = false;
                }
            }
        }

        /// <summary>
        /// 执行数据库操作（同步锁）
        /// </summary>
        /// <param name="operation"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void Execute(Action<LiteDatabase> operation)
        {
            // 防止同一线程重复获取锁导致死锁
            if (IsHoldingLock.Value)
            {
                try
                {
                    operation(_db);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("数据库操作失败", ex);
                }
                return;
            }
            
            lock (Lock)
            {
                IsHoldingLock.Value = true;
                try
                {
                    operation(_db);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("数据库操作失败", ex);
                }
                finally
                {
                    IsHoldingLock.Value = false;
                }
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
                    col.Insert(new SMSDatabaseData(content));
                });
                return true; // 插入成功
            }
            catch (InvalidOperationException)
            {
                return false; // 插入失败
            }
        }
        
        /// <summary>
        /// 释放数据库资源
        /// </summary>
        public void Dispose()
        {
            _db.Dispose();
            IsHoldingLock.Dispose();
        }
    }
}
