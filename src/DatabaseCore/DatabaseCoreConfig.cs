namespace DatabaseCore;

/// <summary>
/// 数据库配置
/// </summary>
public class DatabaseCoreConfig
{
    /// <summary>
    /// 数据库文件路径
    /// </summary>
    public static readonly string LiteDbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.db");
    
    /// <summary>
    /// 管理员数据文件路径
    /// </summary>
    public static readonly string AdminDataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "admin.json");
    
    /// <summary>
    /// 网络加密密钥文件路径
    /// </summary>
    public static readonly string NetworkEncryptionKeyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "network_encryption_key.txt");
}