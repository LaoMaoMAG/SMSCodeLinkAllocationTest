namespace APIModes.ReturnModes;

public class ReturnStringData : ReturnDataBase
{
    /// <summary>
    /// 数据
    /// </summary>
    public string? Data { get; set; }
    
    /// <summary>
    /// AES 加密的 IV
    /// </summary>
    public string? AesIv { get; set; }
}