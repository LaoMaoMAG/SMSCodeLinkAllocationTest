namespace APIModes;

/// <summary>
/// 用户SMS用户设置数据
/// </summary>
// ReSharper disable once InconsistentNaming
public class UserSMSUserSettingsData
{
    /// <summary>
    /// 是否启用用户访问SMS
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public bool IsEnableUserAccessSMS { get; set; }
    
    /// <summary>
    /// 是否启用用户多访问一次SMS
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public bool IsEnableUserMultipleVisitsAtOnceSMS { get; set; }
    
    /// <summary>
    /// 用户访问SMS密钥
    /// </summary>
    public string? UserAccessKey { get; set; }
    
    /// <summary>
    /// 用户访问SMS加密密钥
    /// </summary>
    public string? UserAccessEncryptionKey { get; set; }
    
    /// <summary>
    /// 管理员面板名称
    /// </summary>
    public string? AdminPanelName { get; set; }
}