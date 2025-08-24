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
    /// 私有构造函数
    /// </summary>
    private AdminAuthentication()
    { 
    }
}