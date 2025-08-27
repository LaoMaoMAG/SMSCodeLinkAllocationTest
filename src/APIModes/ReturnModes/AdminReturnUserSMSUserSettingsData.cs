namespace APIModes.ReturnModes;

/// <summary>
/// 用户短信用户设置返回数据
/// </summary>
// ReSharper disable once InconsistentNaming
public class AdminReturnUserSMSUserSettingsData : ReturnDataBase
{
    public UserSMSUserSettingsData? Data { get; set; }
}