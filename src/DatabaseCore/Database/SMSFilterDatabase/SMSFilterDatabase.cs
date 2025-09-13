using LiteDB;

namespace DatabaseCore;

/// <summary>
/// 短信筛选器数据库
/// </summary>
// ReSharper disable once InconsistentNaming
public partial class SMSFilterDatabase
{
    /// <summary>
    /// 单例实例
    /// </summary>
    public static SMSFilterDatabase Instance = new SMSFilterDatabase();
    
    /// <summary>
    /// 分组配置表名称
    /// </summary>
    private const string GroupConfigTableName = "sms_filter";
    
    /// <summary>
    /// 数据库连接
    /// </summary>
    private readonly LiteDatabase _db;
    
    /// <summary>
    /// 缓存数据
    /// </summary>
    private readonly Dictionary<ObjectId, SMSFilterDatabaseData> _cacheData = new();
    
    /// <summary>
    /// 私有构造函数
    /// </summary>
    private SMSFilterDatabase()
    { 
        // 连接数据库
        var connectionString = new ConnectionString(DatabaseCoreConfig.LiteDbFilePath);
        _db = new LiteDatabase(connectionString);
            
        // 创建数据表并设置索引
        var collection = _db.GetCollection<SMSFilterDatabaseData>(GroupConfigTableName);
        collection.EnsureIndex(x => x.Id, true); // 设置唯一索引

        // 更新所有缓存数据
        UpdateAllCacheData();
    }
    
    /// <summary>
    /// 新增筛选器
    /// </summary>
    public bool NewFilter(SMSFilterDatabaseData data)
    {
        try
        {
            var collection = _db.GetCollection<SMSFilterDatabaseData>(GroupConfigTableName);
            collection.Insert(data);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// 删除筛选器
    /// </summary>
    public bool DeleteFilter(ObjectId id)
    {
        try
        {
            var col = _db.GetCollection<SMSFilterDatabaseData>(GroupConfigTableName);
            col.DeleteMany(x => x.Id.Equals(id));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// 修改筛选器
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool UpdateFilter(SMSFilterDatabaseData data)
    {
        try
        {
            var col = _db.GetCollection<SMSFilterDatabaseData>(GroupConfigTableName);
            col.Update(data);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 获取筛选器列表数据
    /// </summary>
    /// <returns></returns>
    public List<SMSFilterDatabaseData>? GetFilterListData()
    {
        try
        {
            var col = _db.GetCollection<SMSFilterDatabaseData>(GroupConfigTableName);
            return col.FindAll().ToList();
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// 更新所有缓存数据
    /// </summary>
    private void UpdateAllCacheData()
    {
        _cacheData.Clear();
        var dataList = GetFilterListData();
        if (dataList == null || dataList.Count == 0) return;
        foreach (var data in dataList) _cacheData.Add(data.Id, data);
    }
    
    /// <summary>
    /// 获取筛选器数据
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public SMSFilterDatabaseData? GetFilterData(ObjectId id)
    {
        try
        {
            if (_cacheData.TryGetValue(id, out var data)) return data;
            var col = _db.GetCollection<SMSFilterDatabaseData>(GroupConfigTableName);
            return col.FindOne(x => x.Id.Equals(id));
        }
        catch (Exception e)
        {
            return null;
        }
    }
}