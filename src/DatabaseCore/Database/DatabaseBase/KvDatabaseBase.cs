using LiteDB;

namespace DatabaseCore.DatabaseBase;

/// <summary>
/// KV数据库基础类
/// </summary>
public abstract class KvDatabaseBase
{
    /// <summary>
    /// 数据库文件路径
    /// </summary>
    protected abstract string DbFilePath { get; init; } 
    
    /// <summary>
    /// 配置表名称
    /// </summary>
    protected abstract string ConfigTableName { get; init; }
    
    /// <summary>
    /// 设置键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <typeparam name="T">值的类型</typeparam>
    protected void SetValue<T>(string key, T value)
    {
        using var db = new LiteDatabase(DbFilePath);
        var col = db.GetCollection<KvDatabaseData<T>>(ConfigTableName);
        col.EnsureIndex(x => x.Key, true); // 确保键的唯一性

        var entry = col.FindOne(x => x.Key == key);
        if (entry != null)
        {
            entry.Value = value;
            entry.UpdateTime = DateTime.Now;
            col.Update(entry);
        }
        else
        {
            entry = new KvDatabaseData<T>
            {
                Key = key,
                Value = value,
                UpdateTime = DateTime.Now
            };
            col.Insert(entry);
        }
    }

    /// <summary>
    /// 获取指定键的值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <typeparam name="T">值的类型</typeparam>
    /// <returns>键对应的值，如果不存在则返回默认值</returns>
    protected T? GetValue<T>(string key, T? defaultValue = default)
    {
        using var db = new LiteDatabase(DbFilePath);
        var col = db.GetCollection<KvDatabaseData<T>>(ConfigTableName);
        var entry = col.FindOne(x => x.Key == key);
        return entry != null ? entry.Value : defaultValue;
    }

    /// <summary>
    /// 检查键是否存在
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>键是否存在</returns>
    protected bool ContainsKey(string key)
    {
        using var db = new LiteDatabase(DbFilePath);
        var col = db.GetCollection<KvDatabaseData<object>>(ConfigTableName);
        return col.Exists(x => x.Key == key);
    }

    /// <summary>
    /// 删除指定键
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否删除成功</returns>
    protected bool RemoveKey(string key)
    {
        using var db = new LiteDatabase(DbFilePath);
        var col = db.GetCollection<KvDatabaseData<object>>(ConfigTableName);
        return col.DeleteMany(x => x.Key == key) > 0;
    }

    /// <summary>
    /// 获取所有键
    /// </summary>
    /// <returns>所有键的列表</returns>
    protected List<string> GetAllKeys(string collectionName)
    {
        using var db = new LiteDatabase(DbFilePath);
        var col = db.GetCollection<KvDatabaseData<object>>(collectionName);
        return col.FindAll().Select(x => x.Key).ToList();
    }

    /// <summary>
    /// KV条目数据结构
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    // ReSharper disable once MemberCanBePrivate.Global
    public class KvDatabaseData<T>
    {
        [BsonId] public ObjectId Id { get; set; } = null!;

        public string Key { get; init; } = null!;

        public T? Value { get; set; }

        public DateTime CreateTime { get; } = DateTime.Now;
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}
