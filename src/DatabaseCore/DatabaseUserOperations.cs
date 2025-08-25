namespace DatabaseCore;

// 数据库用户操作
public partial class DatabaseManager
{
    /// <summary>
    /// 请求短信
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public List<SMSDatabaseData>? RequestSMS(int count)
    {
        try
        {
            return Execute(db =>
            {
                var col = db.GetCollection<SMSDatabaseData>(SMSTableName);

                // 按最后访问时间升序排列，且仅包含启用的记录
                var smsList = col.Query()
                    .Where(x => x.IsEnable == true) // 添加筛选条件
                    .OrderBy(x => x.LastAccessTime)
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
    public string? RequestSMS()
    {
        var smsList = RequestSMS(1);
        return smsList?.FirstOrDefault()?.Content;
    }
}