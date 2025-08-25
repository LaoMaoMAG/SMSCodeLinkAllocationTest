using APIModes.RequestModes;
using APIModes.ReturnModes;
using DatabaseCore;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public partial class AdminRequest
{
    /// <summary>
    /// 添加短信
    /// </summary>
    /// <param name="requestString"></param>
    [HttpPost("AddSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult AddSMS([FromBody] AdminRequestStringDataBase requestString)
    {
        if (!VerifyAdminAccount(requestString)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        if (requestString.Data == null) 
            return BadRequest(new ReturnDataBase { Success = false, Message = "Content 不能为空！" });
        
        var isSuccess = DatabaseManager.Instance.AddSMS(requestString.Data);
        
        return Ok(isSuccess
            ? new ReturnDataBase { Success = true, Message = "添加数据成功！" }
            : new ReturnDataBase { Success = false, Message = "添加数据失败！" });
    }

    /// <summary>
    /// 批量添加短信
    /// </summary>
    /// <param name="requestStringList"></param>
    /// <returns></returns>
    [HttpPost("AddMultipleSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult AddMultipleSMS([FromBody] AdminRequestStringListDataBase requestStringList)
    {
        if (!VerifyAdminAccount(requestStringList)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        if (requestStringList.Data == null) 
            return BadRequest(new ReturnDataBase { Success = false, Message = "Content 不能为空！" });
        
        var isSuccess = DatabaseManager.Instance.AddSMS(requestStringList.Data);
        
        return Ok(isSuccess
            ? new ReturnDataBase { Success = true, Message = "添加数据成功！" }
            : new ReturnDataBase { Success = false, Message = "添加数据失败！" });
    }

    /// <summary>
    /// 获取短信分页数据
    /// </summary>
    [HttpGet("GetSMSPageData")]
    [HttpPost("GetSMSPageData")]
    // ReSharper disable once InconsistentNaming
    public IActionResult GetSMSPageData([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromBody] AdminRequestDataBase request)
    {
        if (!VerifyAdminAccount(request)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        return Ok();
    }
}