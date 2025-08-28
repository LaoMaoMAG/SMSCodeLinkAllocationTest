using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DatabaseCore;

/// <summary>
/// 网络加密工具类（AES + PBKDF2）
/// 支持中文加密/解密，自动处理 IV 和密钥派生
/// </summary>
public class NetworkEncryption
{
    // 盐（salt），建议在配置中固定或随机生成后保存
    // 注意：如果使用随机 salt，需随密文一起传输
    private static readonly byte[] DefaultSalt = "YourUniqueSalt16"u8.ToArray(); // 至少 8 字节

    /// <summary>
    /// 单例实例
    /// </summary>
    private static NetworkEncryption? _instance;

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static NetworkEncryption Instance => _instance ??= new NetworkEncryption();

    /// <summary>
    /// 私有构造函数（单例）
    /// </summary>
    private NetworkEncryption()
    {
    }

    /// <summary>
    /// 加密字符串（支持中文）
    /// </summary>
    /// <param name="data">要加密的明文</param>
    /// <param name="iv">IV</param>
    /// <returns>Base64 编码的密文（包含 IV）</returns>
    public string Encrypt(string data, out string? iv)
    {
        iv = null;
        
        // 如果未设置密钥，则直接返回明文
        if (string.IsNullOrEmpty(ConfigDatabase.Instance.UserAccessEncryptionKey)) return data;
        
        // 生成 16 字节随机 IV
        var ivBytes = RandomNumberGenerator.GetBytes(16);
        // 将 IV 转为 Base64 字符串，用于输出（比如存储或传输）
        iv = Convert.ToBase64String(ivBytes);
        
        // 使用 AES 加密
        var key = GetAesKeyFromPassword(ConfigDatabase.Instance.UserAccessEncryptionKey);
        return AesEncrypt(data, key, ivBytes);
    }

    /// <summary>
    /// 解密字符串
    /// </summary>
    /// <param name="data">Base64 编码的密文</param>
    /// <param name="key">密钥</param>
    /// <param name="iv">IV</param>
    /// <returns>解密后的明文</returns>
    public string Decrypt(string data, string key, string iv)
    {
        if (string.IsNullOrEmpty(ConfigDatabase.Instance.UserAccessEncryptionKey)) 
            return data;
        
        // 应是 Base64 字符串
        var ivBytes = Convert.FromBase64String(iv);
        // 应返回 32 字节用于 AES-256
        var keyBytes = GetAesKeyFromPassword(key);

        return AesDecrypt(data, keyBytes, ivBytes);
    }

    /// <summary>
    /// 获取 AES 密钥（使用 PBKDF2）
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static byte[] GetAesKeyFromPassword(string password)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        // SHA-256 输出正好是 32 字节，适合 AES-256
        return hash;
    }

    /// <summary>
    /// AES 加密
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static string AesEncrypt(string plainText, byte[] key, byte[] iv)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("明文不能为空");
        if (key == null || key.Length != 32) // AES-256 要求 32 字节
            throw new ArgumentException("密钥必须是 32 字节（256 位）");
        if (iv == null || iv.Length != 16) // IV 必须是 16 字节
            throw new ArgumentException("IV 必须是 16 字节");

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aes.CreateEncryptor();

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    /// <summary>
    /// AES 解密
    /// </summary>
    /// <param name="cipherText"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static string AesDecrypt(string cipherText, byte[] key, byte[] iv)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentException("密文不能为空");
        if (key == null || key.Length != 32)
            throw new ArgumentException("密钥必须是 32 字节");
        if (iv == null || iv.Length != 16)
            throw new ArgumentException("IV 必须是 16 字节");

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aes.CreateDecryptor();

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (MemoryStream ms = new MemoryStream(cipherBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}