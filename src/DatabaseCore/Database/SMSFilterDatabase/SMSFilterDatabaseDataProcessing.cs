using LiteDB;

namespace DatabaseCore;

// 短信筛选器数据库 数据处理
// ReSharper disable once InconsistentNaming
public partial class SMSFilterDatabase
{
    /// <summary>
    /// 筛选数据
    /// </summary>
    /// <param name="query">要筛选的查询对象</param>
    /// <param name="filter">筛选器属性</param>
    /// <returns>筛选后的查询对象，如果出错则返回null</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public ILiteQueryable<SMSDatabaseData>? FilterData(ILiteQueryable<SMSDatabaseData> query, SMSFilterDatabaseData filter)
    {
        try
        {
            // 如果筛选器不存在或者未启用，则返回原始查询对象
            if (filter is not { IsEnabled: true }) return query;

            // 启用状态
            query = filter.EnabledStatus switch
            {
                EnumSMSFilterEnabledStatus.Enabled => query.Where(x => x.IsEnable == true),
                EnumSMSFilterEnabledStatus.Disabled => query.Where(x => x.IsEnable == false),
                _ => query
            };

            // 最大访问次数
            if (filter.MaxAccessCount != null)
                query = query.Where(x => x.AccessCount <= filter.MaxAccessCount);

            // 最小访问次数
            if (filter.MinAccessCount != null)
                query = query.Where(x => x.AccessCount >= filter.MinAccessCount);

            // 最大创建时间
            if (filter.MaxCreateTime != null)
                query = query.Where(x => x.CreateTime <= filter.MaxCreateTime);

            // 最小创建时间
            if (filter.MinCreateTime != null)
                query = query.Where(x => x.CreateTime >= filter.MinCreateTime);

            // 最大最后访问时间
            if (filter.MaxLastAccessTime != null)
                query = query.Where(x => x.LastAccessTime <= filter.MaxLastAccessTime);

            // 最小最后访问时间
            if (filter.MinLastAccessTime != null)
                query = query.Where(x => x.LastAccessTime >= filter.MinLastAccessTime);

            // 分组列表
            if (filter.GroupList != null)
                query = filter.GroupList.Aggregate(query, (current, groupId)
                    => current.Where(x => x.GroupId == groupId));

            // 返回查询对象
            return query;
        }
        catch (Exception)
        {
            // 出错则返回 null
            return null;
        }
    }

    /// <summary>
    /// 筛选数据
    /// </summary>
    /// <param name="query">要筛选的查询对象</param>
    /// <param name="filterId">筛选器 ID</param>
    /// <returns>筛选后的查询对象，如果出错则返回null</returns>
    public ILiteQueryable<SMSDatabaseData>? FilterData(ILiteQueryable<SMSDatabaseData> query, ObjectId filterId)
    {
        var filter = GetFilterData(filterId);
        return filter is null ? null : FilterData(query, filter);
    }
}