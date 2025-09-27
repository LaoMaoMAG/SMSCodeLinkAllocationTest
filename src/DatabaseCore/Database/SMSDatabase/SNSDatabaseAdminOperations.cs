namespace DatabaseCore;

// 数据库管理操作
// ReSharper disable once InconsistentNaming
public sealed partial class SMSDatabase
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
                col.EnsureIndex(x => x.Content, true);
            
                // 批量插入，减少事务开销
                var batchData = contentList
                    .Where(content => !string.IsNullOrWhiteSpace(content))
                    .Distinct() // 去重，避免唯一索引冲突
                    .Select(content => new SMSDatabaseData(content))
                    .ToList();
                
                if (batchData.Count > 0)
                {
                    col.InsertBulk(batchData); // 使用批量插入
                }
            });
            return true;
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
                col.EnsureIndex(x => x.Content, true); // 设置唯一索引
                col.DeleteMany(x => content.Contains(x.Content));
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
                col.EnsureIndex(x => x.Content, true); // 设置唯一索引
                var data = col.Find(x => true)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                return data;
            });
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
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
        }
        catch (InvalidOperationException)
        {
            return 0;
        }
    }
    
    /// <summary>
    /// 获取禁用短信总数
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int GetDisableSMSCount()
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                return col.FindAll().Count(x => !x.IsEnable);
            });
        }
        catch (InvalidOperationException)
        {
            return 0;
        }
    }

    /// <summary>
    /// 设置短信是否启用
    /// </summary>
    /// <param name="content">要禁用的短信内容列表</param>
    /// <param name="isEnable"></param>
    /// <returns>操作是否成功</returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public bool SetSMSIsEnable(List<string> content, bool isEnable)
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                col.EnsureIndex(x => x.Content, true); // 设置唯一索引
                // 查找需要禁用的短信并更新其 IsEnable 状态
                var smsToUpdate = col.Find(x => content.Contains(x.Content)).ToList();
                foreach (var sms in smsToUpdate)
                {
                    sms.IsEnable = isEnable;
                    var updateResult = col.Update(sms); // 检查更新结果
                    if (!updateResult) // 如果更新失败
                        throw new InvalidOperationException("Failed to update SMS record");
                }
                return true; // 确保Execute方法返回成功标识
            });
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    /// <summary>
    /// 设置短信是否启用
    /// </summary>
    /// <param name="content">要禁用的短信内容</param>
    /// <param name="isEnable"></param>
    /// <returns>操作是否成功</returns>
    // ReSharper disable once InconsistentNaming
    public bool SetSMSIsEnable(string content, bool isEnable)
    {
        return SetSMSIsEnable([content], isEnable);
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
                col.EnsureIndex(x => x.Content, true); // 设置唯一索引
                return col.FindAll().Select(x => x.Content).ToList();
            });
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}