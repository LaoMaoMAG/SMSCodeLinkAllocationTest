namespace DatabaseCore;

/// <summary>
/// 网络加密
/// </summary>
public class NetworkEncryption
{
    /// <summary>
    /// 单例实例
    /// </summary>
    private static NetworkEncryption? _instance;

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static NetworkEncryption Instance => _instance ??= new NetworkEncryption();

    /// <summary>
    /// 网络加密密钥
    /// </summary>
    private string? _networkEncryptionKey;
    
    /// <summary>
    /// 私有构造函数
    /// </summary>
    private NetworkEncryption()
    { 
        // 检测管理员账户文件是否存在
        if (File.Exists(DatabaseCoreConfig.NetworkEncryptionKeyFilePath))
        {
            _networkEncryptionKey = File.ReadAllText(DatabaseCoreConfig.AdminDataFilePath);
        }
    }

    /// <summary>
    /// 设置网络加密密钥
    /// </summary>
    /// <param name="key">密钥：""（默认）为随机，null为不加密</param>
    public void SetEncryptionKey(string? key = "")
    {
        if (key == "") _networkEncryptionKey = GenerateRandomString(32);
        if (_networkEncryptionKey == null) key = "";
        _networkEncryptionKey = key;
        File.WriteAllText(DatabaseCoreConfig.NetworkEncryptionKeyFilePath, _networkEncryptionKey);
    }
    
    /// <summary>
    /// 加密数据
    /// </summary>
    /// <param name="data">要加密的数据</param>
    /// <returns>加密完成的数据</returns>
    public string? Encrypt(string data)
    {
        if (string.IsNullOrEmpty(_networkEncryptionKey)) return data;
        return _networkEncryptionKey == null ? null : RC4.Encrypt(_networkEncryptionKey, data);
    }
    
    /// <summary>
    /// 解密数据
    /// </summary>
    /// <param name="data">要解密的数据</param>
    /// <returns>解密完成的数据</returns>
    public string? Decrypt(string data)
    {
        if (string.IsNullOrEmpty(_networkEncryptionKey)) return data;
        return _networkEncryptionKey == null ? null : RC4.Decrypt(_networkEncryptionKey, data);
    }
    
    /// <summary>
    /// 生成指定长度的随机字符串
    /// </summary>
    /// <param name="length">字符串长度</param>
    /// <returns>随机字符串</returns>
    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }
        
        return new string(result);
    }
}