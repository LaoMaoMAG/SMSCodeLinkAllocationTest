using DatabaseCore.DatabaseBase;

namespace DatabaseCore;

/// <summary>
/// 配置数据库管理器
/// </summary>
public class ConfigDatabase : KvDatabaseBase
{
    /// <summary>
    /// 单例实例
    /// </summary>
    private static ConfigDatabase? _instance;

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static ConfigDatabase Instance => _instance ??= new ConfigDatabase();

    /// <summary>
    /// 数据库文件路径
    /// </summary>
    protected override string DbFilePath { get; init; } = DatabaseCoreConfig.LiteDbFilePath;

    /// <summary>
    /// 配置表名
    /// </summary>
    protected override string ConfigTableName { get; init; } = "config";

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private ConfigDatabase()
    {
    }

    /// <summary>
    /// 用户API访问次数
    /// </summary>
    public int UserApiAccessCount
    {
        get => !ContainsKey("user_api_access_count") ? 0 : GetValue<int>("user_api_access_count");
        set => SetValue("user_api_access_count", value);
    }

    /// <summary>
    /// 是否允许用户访问短信
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public bool IsEnableUserAccessSMS
    {
        get
        {
            if (!ContainsKey("is_enable_user_access_sms"))
                SetValue("is_enable_user_access_sms", true); // 默认值
            return GetValue<bool>("is_enable_user_access_sms");
        }
        set => SetValue("is_enable_user_access_sms", value);
    }
    
    /// <summary>
    /// 是否允许用户同时访问多个短信
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public bool IsEnableUserMultipleVisitsAtOnceSMS
    {
        get
        {
            if (!ContainsKey("is_enable_user_multiple_visits_at_once"))
                SetValue("is_enable_user_multiple_visits_at_once", true); // 默认值
            return GetValue<bool>("is_enable_user_multiple_visits_at_once");
        }
        set => SetValue("is_enable_user_multiple_visits_at_once", value);
    }

    /// <summary>
    /// 用户访问密钥
    /// </summary>
    public string? UserAccessKey
    {
        get => GetValue<string?>("user_access_key");
        set => SetValue("user_access_key", value);
    }

    /// <summary>
    /// 用户访问加密密钥
    /// </summary>
    public string? UserAccessEncryptionKey
    {
        get => GetValue<string?>("user_access_encryption_key");
        set => SetValue("user_access_encryption_key", value);
    }

    /// <summary>
    /// 管理员面板名称
    /// </summary>
    public string? AdminPanelName
    {
        get => GetValue<string?>("admin_panel_name");
        set => SetValue("admin_panel_name", value);
    }
}