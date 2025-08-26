namespace DatabaseCore;

// 数据库管理操作
// ReSharper disable once InconsistentNaming
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
    /// <param name="content"></param>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public bool DeleteSMS(List<string> content)
    {
        try
        {
            Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                col.EnsureIndex(x => x.Content, true); // 唯一
                col.DeleteMany(x => content.Contains(x.Content)); // 修改这里
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
    public bool DeleteSMS(string content)
    {
        var smsList = DeleteSMS([content]);
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

    /// <summary>
    /// 禁用短信
    /// </summary>
    /// <param name="content">要禁用的短信内容列表</param>
    /// <returns>操作是否成功</returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public bool DisableSMS(List<string> content)
    {
        try
        {
            Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                col.EnsureIndex(x => x.Content, true); // 确保唯一索引
            
                // 查找需要禁用的短信并更新其 IsEnable 状态
                var smsToUpdate = col.Find(x => content.Contains(x.Content)).ToList();
                foreach (var sms in smsToUpdate)
                {
                    sms.IsEnable = false;
                    col.Update(sms);
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
    /// 禁用单条短信
    /// </summary>
    /// <param name="content">要禁用的短信内容</param>
    /// <returns>操作是否成功</returns>
    // ReSharper disable once InconsistentNaming
    public bool DisableSMS(string content)
    {
        return DisableSMS([content]);
    }

    /// <summary>
    /// 获取所有短信内容
    /// </summary>
    /// <returns>所有短信内容列表</returns>
    // ReSharper disable once InconsistentNaming
    public List<string>? GetAllSMSContent()
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                return col.FindAll().Select(x => x.Content).ToList();
            });
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}