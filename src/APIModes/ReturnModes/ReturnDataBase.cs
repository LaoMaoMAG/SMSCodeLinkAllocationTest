namespace APIModes.ReturnModes;

public class ReturnDataBase
{
    /// <summary>
    /// 请求结果
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }
}