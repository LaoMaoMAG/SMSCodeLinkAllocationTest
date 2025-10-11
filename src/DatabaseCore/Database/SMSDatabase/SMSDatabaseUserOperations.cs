namespace DatabaseCore;

// 数据库用户操作
// ReSharper disable once InconsistentNaming
public sealed partial class SMSDatabase
{
    /// <summary>
    /// 请求短信
    /// </summary>
    /// <param name="count">请求的短信数量</param>
    /// <param name="isTimeDescendingOrder">是否按时间降序排列，默认为false（升序）</param>
    /// <param name="filter">筛选器</param>
    /// <returns>短信数据列表</returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public List<SMSDatabaseData>? RequestSMS(
        int count,
        bool isTimeDescendingOrder = false,
        SMSFilterDatabaseData? filter = null
    )
    {
        try
        {
            // 如果筛选器不存在，则返回原始查询对象
            filter ??= new SMSFilterDatabaseData("默认");
            
            // 执行数据库操作
            return Execute(db =>
            {
                // 获取集合
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);
                col.EnsureIndex(x => x.Content, true); // 设置唯一索引
                
                // 创建查询对象
                var query = col.Query(); 
                
                // 筛选数据
                query = SMSFilterDatabase.Instance.FilterData(query, filter); // 筛选器
                if (query == null) throw new Exception("筛选器数据处理出错"); // 如果筛选器处理出错，则抛出异常
                
                // 按最后访问时间排序，根据isTimeDescendingOrder参数决定升序或降序
                query = isTimeDescendingOrder
                    ? query.OrderByDescending(x => x.LastAccessTime)
                    : query.OrderBy(x => x.LastAccessTime);
                
                // 获取结果
                var smsList = query
                    .Limit(count)
                    .ToList();

                // 更新访问次数和最后访问时间
                foreach (var sms in smsList)
                {
                    sms.AccessCount++;
                    sms.LastAccessTime = DateTime.Now;
                    col.Update(sms);
                }

                // 返回结果
                return smsList.Count == 0 ? null : smsList;
            });
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
    
    /// <summary>
    /// 请求短信
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public string? RequestSMS(
        bool isTimeDescendingOrder = false,
        SMSFilterDatabaseData? filter = null
    )
    {
        var smsList = RequestSMS(1, isTimeDescendingOrder, filter);
        return smsList?.FirstOrDefault()?.Content;
    }
}