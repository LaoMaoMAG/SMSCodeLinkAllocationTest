using LiteDB;

namespace DatabaseCore;

// 数据库管理操作
public partial class SMSDatabaseManager
{
    /// <summary>
    /// 添加短信
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public bool AddSMS(List<string> contentList)
    {
        try
        {
            Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                col.EnsureIndex(x => x.Content, true); // 唯一
                
                foreach (var content in contentList)
                {
                    try
                    {
                        col.Insert(new SMSDatabaseData(content));
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
            });
            return true; // 插入成功
        }
        catch (InvalidOperationException)
        {
            return false; // 插入失败
        }
    }
    
    /// <summary>
    /// 删除短信
    /// </summary>
    /// <param name="objectId"></param>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public bool DeleteSMS(List<ObjectId> objectId)
    {
        try
        {
            Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                foreach (var id in objectId)
                {
                    col.Delete(id);
                }
            });
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    /// <summary>
    /// 删除短信
    /// </summary>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    public bool DeleteSMS(ObjectId objectId)
    {
        var smsList = DeleteSMS([objectId]);
        return smsList;
    }
    
    /// <summary>
    /// 删除所有短信
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public bool DeleteAllSMS()
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                col.DeleteAll();
                return true;
            });
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
    
    /// <summary>
    /// 获取短信分页数据
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    public List<SMSDatabaseData>? GetSMSPageData(int pageIndex, int pageSize)
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                var data = col.Find(x => true)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                return data;
            });
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
    
    /// <summary>
    /// 获取短信总数
    /// </summary>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    public int GetSMSTotal()
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                return col.Count();
            });
            return 0;
        }
        catch (InvalidOperationException)
        {
            return 0;
        }
    }

    /// <summary>
    /// 获取访问总数
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int GetAccessSMSTotal()
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                return col.FindAll().Sum(x => x.AccessCount);
            });
            return 0;
        }
        catch (InvalidOperationException)
        {
            return 0;
        }
    }
    
    /// <summary>
    /// 获取启用短信总数
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int GetEnableSMSCount()
    {
        try
        {
            Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                return col.FindAll().Count(x => x.IsEnable);
            });
            return 0;
        }
        catch (InvalidOperationException)
        {
            return 0;
        }
    }
}