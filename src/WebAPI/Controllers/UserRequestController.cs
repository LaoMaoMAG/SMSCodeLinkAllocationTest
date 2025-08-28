using System.Text.Json;
using APIModes.RequestModes;
using APIModes.ReturnModes;
using DatabaseCore;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserRequestController : ControllerBase
{
    /// <summary>
    /// 用户访问验证
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool UserAccessVerification(UserRequestDataBase data)
    {
        if (string.IsNullOrEmpty(ConfigDatabase.Instance.UserAccessKey)) return true;
        return ConfigDatabase.Instance.UserAccessKey == data.AccessKey;
    }
    
    /// <summary>
    /// 获取单个短信
    /// </summary>
    /// <returns></returns>
    [HttpGet("RequestSingleSMS")]
    [HttpPost("RequestSingleSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult RequestSingleSMS([FromBody] UserRequestDataBase request)
    {
        ConfigDatabase.Instance.UserApiAccessCount++;

        // 解密请求数据
        if (!UserAccessVerification(request))
        {
            return Unauthorized(new ReturnDataBase
            {
                Success = false,
                Message = "访问验证未通过！"
            });
        }
        
        if(!ConfigDatabase.Instance.IsEnableUserAccessSMS) 
        {
            // 访问短信未启用，返回403 Forbidden状态码
            return StatusCode(403, new ReturnStringData
            {
                Success = false,
                Message = "访问短信未启用！"
            });
        }
        
        var data = SMSDatabaseManager.Instance.RequestSMS();
        
        if(data == null) return Ok(new ReturnStringData
        {
            Success = false,
            Message = "获取失败！"
        });
        
        return Ok(new ReturnStringData
        {
            Success = true,
            Message = "获取成功！",
            Data = NetworkEncryption.Instance.Encrypt(data)
        });
    }

    /// <summary>
    /// 获取多个短信
    /// </summary>
    /// <param name="count">获取数量</param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("RequestMultipleSMS")]
    [HttpPost("RequestMultipleSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult RequestMultipleSMS([FromQuery] string count, [FromBody] UserRequestDataBase request)
    {
        ConfigDatabase.Instance.UserApiAccessCount++;
    
        // 解密请求数据
        if (!UserAccessVerification(request))
        {
            return Unauthorized(new ReturnDataBase
            {
                Success = false,
                Message = "访问验证未通过！"
            });
        }
        
        if(!ConfigDatabase.Instance.IsEnableUserAccessSMS) 
        {
            // 访问短信未启用，返回403 Forbidden状态码
            return StatusCode(403, new ReturnStringData
            {
                Success = false,
                Message = "访问短信未启用！"
            });
        }
        
        if(!ConfigDatabase.Instance.IsEnableUserMultipleVisitsAtOnceSMS) 
        {
            // 访问短信未启用，返回403 Forbidden状态码
            return StatusCode(403, new ReturnStringData
            {
                Success = false,
                Message = "单次请求访问多个短信未启用！"
            });
        }
        
        var data = SMSDatabaseManager.Instance.RequestSMS();
    
        if(data == null) return Ok(new ReturnStringData
        {
            Success = false,
            Message = "获取失败！"
        });
        
        // 将data对象转换为JSON字符串
        var jsonData = System.Text.Json.JsonSerializer.Serialize(data);
    
        return Ok(new ReturnStringData
        {
            Success = true,
            Message = "获取成功！",
            Data = NetworkEncryption.Instance.Encrypt(jsonData)
        });
    }
}