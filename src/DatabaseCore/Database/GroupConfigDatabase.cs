using LiteDB;

namespace DatabaseCore;

/// <summary>
/// 分组配置数据库
/// </summary>
public class GroupConfigDatabase
{
    /// <summary>
    /// 分组配置数据库数据
    /// </summary>
    public class GroupConfigDatabaseData
    {
        /// <summary>
        /// 分组 ID
        /// </summary>
        public ObjectId Id { get; set; } = null!;
        
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; } = null!;
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; } = true;
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static GroupConfigDatabase Instance = new GroupConfigDatabase();
    
    /// <summary>
    /// 分组配置表名称
    /// </summary>
    private const string GroupConfigTableName = "group_config";
    
    /// <summary>
    /// 数据库连接
    /// </summary>
    private readonly LiteDatabase _db;
    
    /// <summary>
    /// 私有构造函数
    /// </summary>
    private GroupConfigDatabase()
    {
        // 连接数据库
        var connectionString = new ConnectionString(DatabaseCoreConfig.LiteDbFilePath);
        _db = new LiteDatabase(connectionString);
            
        // 创建数据表并设置索引
        var collection = _db.GetCollection<GroupConfigDatabaseData>(GroupConfigTableName);
        collection.EnsureIndex(x => x.Id, true); // 设置唯一索引
    }
    
    /// <summary>
    /// 新增分组
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public bool NewGroup(string groupName)
    {
        try
        {
            var col = _db.GetCollection<GroupConfigDatabaseData>(GroupConfigTableName);
            col.Insert(new GroupConfigDatabaseData { GroupName = groupName });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 删除分组
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public bool DeleteGroup(string groupName)
    {
        try
        {
            var col = _db.GetCollection<GroupConfigDatabaseData>(GroupConfigTableName);
            col.DeleteMany(x => groupName.Contains(x.GroupName));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 修改分组名称
    /// </summary>
    /// <param name="oldGroupName"></param>
    /// <param name="newGroupName"></param>
    /// <returns></returns>
    public bool ModifyGroupName(string oldGroupName, string newGroupName)
    {
        try
        {
            var col = _db.GetCollection<GroupConfigDatabaseData>(GroupConfigTableName);
            var recordsToUpdate = col.Find(x => x.GroupName == oldGroupName);
        
            foreach (var record in recordsToUpdate)
            {
                record.GroupName = newGroupName;
                col.Update(record);
            }
        
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// 获取分组数据列表
    /// </summary>
    /// <returns></returns>
    public List<GroupConfigDatabaseData>? GetGroupListData()
    {
        try
        {
            var col = _db.GetCollection<GroupConfigDatabaseData>(GroupConfigTableName);
            return col.FindAll().ToList();
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    /// <summary>
    /// 获取分组数据
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public GroupConfigDatabaseData? GetGroupData(string groupName)
    {
        try
        {
            var col = _db.GetCollection<GroupConfigDatabaseData>(GroupConfigTableName);
            return col.FindOne(x => x.GroupName == groupName);
        }
        catch (Exception)
        {
            return null;
        }
    }
}