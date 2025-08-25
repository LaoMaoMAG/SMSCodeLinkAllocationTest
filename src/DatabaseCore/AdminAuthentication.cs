using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DatabaseCore;

/// <summary>
/// 管理员身份验证
/// </summary>
public class AdminAuthentication
{
    /// <summary>
    /// 单例实例
    /// </summary>
    private static AdminAuthentication? _instance;

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static AdminAuthentication Instance => _instance ??= new AdminAuthentication();

    /// <summary>
    /// 是否初始化过管理员账户
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool IsAdminAccountInit { get; private set; }

    /// <summary>
    /// 管理员用户名
    /// </summary>
    private string? _username = null;

    /// <summary>
    /// 管理员密码哈希值
    /// </summary>
    private string? _passwordHash = null;

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private AdminAuthentication()
    {
        // 检测管理员账户文件是否存在
        if (File.Exists(DatabaseCoreConfig.AdminDataFilePath))
        {
            IsAdminAccountInit = true;
            var jsonString = File.ReadAllText(DatabaseCoreConfig.AdminDataFilePath);
            var person = JsonSerializer.Deserialize<AdminAccountData>(jsonString);
            _username = person?.Username;
            _passwordHash = person?.PasswordHash;
        }
        else
        {
            IsAdminAccountInit = false;
        }
    }

    /// <summary>
    /// 初始化管理员账户
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    public bool InitAdminAccount(string username, string password)
    {
        if (IsAdminAccountInit) return false;
        
        // 计算密码的 SHA-256 哈希值
        var passwordHash = ComputeSha256Hash(password);
        
        // 创建 JSON 数据
        var json = JsonSerializer.Serialize(new AdminAccountData(
            username,
            passwordHash
        ));
        
        // 写入文件
        File.WriteAllText(DatabaseCoreConfig.AdminDataFilePath, json);
        
        // 更新属性
        IsAdminAccountInit = true;
        _username = username;
        _passwordHash = passwordHash;
        
        return true;
    }

    /// <summary>
    /// 验证管理员账户
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <returns>验证结果</returns>
    public bool VerifyAdminAccount(string username, string password)
    {
        return VerifyAdminUsername(username) && VerifyAdminPassword(password);
    }

    /// <summary>
    /// 验证管理员用户名
    /// </summary>
    /// <param name="username">用户名</param>
    /// <returns>验证结果</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public bool VerifyAdminUsername(string username)
    {
        return _username == username;
    }
    
    /// <summary>
    /// 验证管理员密码
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns>验证结果</returns>
    public bool VerifyAdminPassword(string password)
    {
        return _passwordHash == ComputeSha256Hash(password);
    }
    
    /// <summary>
    /// 计算输入字符串的 SHA-256 哈希值
    /// </summary>
    /// <param name="rawData">要哈希的字符串</param>
    /// <returns>SHA-256 哈希的十六进制字符串</returns>
    private static string ComputeSha256Hash(string rawData)
    {
        // 创建 SHA256 算法实例
        using var sha256Hash = SHA256.Create();
        // 将输入字符串转换为字节数组（使用 UTF-8 编码，支持中文）
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // 将字节数组转换为十六进制字符串
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2")); // x2 表示小写十六进制，两位
        }

        return builder.ToString();
    }
}