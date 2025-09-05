namespace DatabaseCore;

// 数据库用户操作
// ReSharper disable once InconsistentNaming
public partial class SMSDatabaseManager
{
    /// <summary>
    /// 请求短信
    /// </summary>
    /// <param name="count">请求的短信数量</param>
    /// <param name="isTimeDescendingOrder">是否按时间降序排列，默认为false（升序）</param>
    /// <returns>短信数据列表</returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public List<SMSDatabaseData>? RequestSMS(int count, bool isTimeDescendingOrder = false)
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);

                // 根据isTimeDescendingOrder参数决定排序方向
                var query = col.Query()
                    .Where(x => x.IsEnable == true); // 添加筛选条件

                // 按最后访问时间排序，根据isTimeDescendingOrder参数决定升序或降序
                query = isTimeDescendingOrder
                    ? query.OrderByDescending(x => x.LastAccessTime)
                    : query.OrderBy(x => x.LastAccessTime);

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
    public string? RequestSMS(bool isTimeDescendingOrder = false)
    {
        var smsList = RequestSMS(1, isTimeDescendingOrder);
        return smsList?.FirstOrDefault()?.Content;
    }
}