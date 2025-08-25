using APIModes.RequestModes;
using APIModes.ReturnModes;
using DatabaseCore;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public partial class AdminRequest : ControllerBase
{
    /// <summary>
    /// 验证管理员账号
    /// </summary>
    /// <param name="dataBase">管理员数据</param>
    /// <returns></returns>
    private static bool VerifyAdminAccount(AdminRequestDataBase dataBase)
    {
        if (dataBase.Username == null || dataBase.Password == null) return false;
        var isSuccess = AdminAuthentication.Instance.VerifyAdminAccount(dataBase.Username, dataBase.Password);
        return isSuccess;
    }

    /// <summary>
    /// 管理员账号验证
    /// </summary>
    [HttpPost("AccountAuthentication")]
    public IActionResult AccountAuthentication([FromBody] AdminRequestDataBase request)
    {
        // 验证数据
        if (request.Username == null || request.Password == null || request.Username == "" || request.Password == "")
            return BadRequest(new ReturnIntData { Success = false, Message = "用户名或密码不能为空！", Data = -1 });

        // 验证账号
        if (VerifyAdminAccount(request))
            return Ok(new ReturnIntData { Success = true, Message = "管理员认证成功！", Data = 1 });

        // 验证用户名
        if (!AdminAuthentication.Instance.VerifyAdminUsername(request.Username))
            return Unauthorized(new ReturnIntData { Success = false, Message = "用户名不存在！", Data = -2 });

        // 验证密码
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!AdminAuthentication.Instance.VerifyAdminPassword(request.Password))
            return Unauthorized(new ReturnIntData { Success = false, Message = "密码错误！", Data = -3 });

        // 验证失败
        return Unauthorized(new ReturnIntData { Success = false, Message = "未知错误！", Data = 0 });
    }

    /// <summary>
    /// 初始化管理员账户
    /// </summary>
    [HttpPost("InitAdminAccount")]
    public IActionResult InitAdminAccount([FromBody] AdminRequestInitAccountData request)
    {
        if (request.Username == null || request.Password == null || request.Username == "" || request.Password == "")
            return BadRequest(new ReturnDataBase { Success = false, Message = "用户名或密码不能为空！" });

        return Ok(AdminAuthentication.Instance.InitAdminAccount(request.Username, request.Password)
            ? new ReturnDataBase { Success = true, Message = "初始化成功！" }
            : new ReturnDataBase { Success = false, Message = "初始化失败！可能是管理员账户已存在！" });
    }

    /// <summary>
    /// 获取管理员账户初始化状态
    /// </summary>
    [HttpGet("GetAdminAccountInitStatus")]
    public IActionResult GetAdminAccountInitStatus()
    {
        return Ok(!AdminAuthentication.Instance.IsAdminAccountInit
            ? new ReturnDataBase { Success = false, Message = "管理员账户未初始化！" }
            : new ReturnDataBase { Success = true, Message = "管理员账户已初始化！" });
    }
}