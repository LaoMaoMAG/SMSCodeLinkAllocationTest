using LiteDB;

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
    /// <param name="accessRestriction">访问限制</param>
    /// <param name="isMaxMode">访问限制模式</param>
    /// <returns>短信数据列表</returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public List<SMSDatabaseData>? RequestSMS(
        int count,
        bool isTimeDescendingOrder = false,
        int accessRestriction = 0,
        bool isMaxMode = false
    )
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);

                ILiteQueryable<SMSDatabaseData> query;

                // 根据accessRestriction和isMaxMode参数构建查询条件
                if (accessRestriction <= 0)
                {
                    // 无访问限制时，只筛选启用的短信
                    query = col.Query()
                        .Where(x => x.IsEnable == true);
                }
                else
                {
                    // 有访问限制时，根据isMaxMode决定筛选条件
                    query = isMaxMode
                        ? col.Query().Where(x => x.IsEnable == true && x.AccessCount > accessRestriction)
                        : col.Query().Where(x => x.IsEnable == true && x.AccessCount < accessRestriction);
                }

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
    public string? RequestSMS(
        bool isTimeDescendingOrder = false,
        int accessRestriction = 0,
        bool isMaxMode = false
    )
    {
        var smsList = RequestSMS(1, isTimeDescendingOrder, accessRestriction, isMaxMode);
        return smsList?.FirstOrDefault()?.Content;
    }
}