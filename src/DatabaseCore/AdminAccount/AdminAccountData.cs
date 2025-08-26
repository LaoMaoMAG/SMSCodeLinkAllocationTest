namespace DatabaseCore;

/// <summary>
/// 管理员账户数据
/// </summary>
public class AdminAccountData(string username, string passwordHash)
{
    public string Username { get; set; } = username;
    public string PasswordHash { get; set; } = passwordHash;
}