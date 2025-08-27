namespace APIModes.RequestModes;

/// <summary>
/// 用户短信设置
/// </summary>
// ReSharper disable once InconsistentNaming
public class AdminRequestUserSMSUserSettingsData : AdminRequestDataBase
{
    public UserSMSUserSettingsData? Data { get; set; }
}