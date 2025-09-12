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
    /// <param name="filterId">筛选器ID</param>
    /// <returns>筛选后的查询对象，如果出错则返回null</returns>
    public ILiteQueryable<SMSDatabaseData>? FilterData(ILiteQueryable<SMSDatabaseData> query, ObjectId filterId)
    {
        try
        {
            // 获取筛选器
            var filterData = GetFilterData(filterId);
            
            // 如果筛选器不存在或者未启用，则返回原始查询对象
            if (filterData is not { IsEnabled: true }) return query;

            // 启用状态
            query = filterData.EnabledStatus switch
            {
                EnumSMSFilterEnabledStatus.Enabled => query.Where(x => x.IsEnable == true),
                EnumSMSFilterEnabledStatus.Disabled => query.Where(x => x.IsEnable == false),
                _ => query
            };

            // 最大访问次数
            if (filterData.MaxAccessCount != null)
                query = query.Where(x => x.AccessCount <= filterData.MaxAccessCount);

            // 最小访问次数
            if (filterData.MinAccessCount != null)
                query = query.Where(x => x.AccessCount >= filterData.MinAccessCount);

            // 最大创建时间
            if (filterData.MaxCreateTime != null)
                query = query.Where(x => x.CreateTime <= filterData.MaxCreateTime);

            // 最小创建时间
            if (filterData.MinCreateTime != null)
                query = query.Where(x => x.CreateTime >= filterData.MinCreateTime);

            // 最大最后访问时间
            if (filterData.MaxLastAccessTime != null)
                query = query.Where(x => x.LastAccessTime <= filterData.MaxLastAccessTime);

            // 最小最后访问时间
            if (filterData.MinLastAccessTime != null)
                query = query.Where(x => x.LastAccessTime >= filterData.MinLastAccessTime);

            // 分组列表
            if (filterData.GroupList != null)
                query = filterData.GroupList.Aggregate(query, (current, groupId)
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
}