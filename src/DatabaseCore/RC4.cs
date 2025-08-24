using System.Text;

namespace DatabaseCore;

/// <summary>
/// RC4 加密解密
/// </summary>
// ReSharper disable once InconsistentNaming
public class RC4
{
    public static string Encrypt(string key, string data)
    {
        var unicode = Encoding.Unicode;

        return Convert.ToBase64String(Encrypt(unicode.GetBytes(key), unicode.GetBytes(data)));
    }

    public static string Decrypt(string key, string data)
    {
        var unicode = Encoding.Unicode;

        return unicode.GetString(Encrypt(unicode.GetBytes(key), Convert.FromBase64String(data)));
    }

    public static byte[] Encrypt(byte[] key, byte[] data)
    {
        return EncryptOutput(key, data).ToArray();
    }

    public static byte[] Decrypt(byte[] key, byte[] data)
    {
        return EncryptOutput(key, data).ToArray();
    }

    private static byte[] EncryptInitalize(byte[] key)
    {
        var s = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();

        for (int i = 0, j = 0; i < 256; i++)
        {
            j = (j + key[i % key.Length] + s[i]) & 255;

            Swap(s, i, j);
        }

        return s;
    }

    private static IEnumerable<byte> EncryptOutput(byte[] key, IEnumerable<byte> data)
    {
        var s = EncryptInitalize(key);

        var i = 0;
        var j = 0;

        return data.Select((b) =>
        {
            i = (i + 1) & 255;
            j = (j + s[i]) & 255;

            Swap(s, i, j);

            return (byte)(b ^ s[(s[i] + s[j]) & 255]);
        });
    }

    private static void Swap(byte[] s, int i, int j)
    {
        (s[i], s[j]) = (s[j], s[i]);
    }
}