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
    [HttpPost("RequestSingleSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult RequestSingleSMS(
        [FromBody] UserRequestDataBase request,
        [FromQuery] bool isTimeDescendingOrder = false
    )
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

        if (!ConfigDatabase.Instance.IsEnableUserAccessSMS)
        {
            // 访问短信未启用，返回403 Forbidden状态码
            return StatusCode(403, new ReturnStringData
            {
                Success = false,
                Message = "访问短信未启用！"
            });
        }

        var data = SMSDatabase.Instance.RequestSMS(isTimeDescendingOrder);

        if (data == null)
            return Ok(new ReturnStringData
            {
                Success = false,
                Message = "获取失败！"
            });

        var encryptData = NetworkEncryption.Instance.Encrypt(data, out var aesIv);

        return Ok(new ReturnStringData
        {
            Success = true,
            Message = "获取成功！",
            Data = encryptData,
            AesIv = aesIv
        });
    }

    /// <summary>
    /// 获取多个短信
    /// </summary>
    /// <param name="count">获取数量</param>
    /// <param name="request"></param>
    /// <param name="isTimeDescendingOrder"></param>
    /// <returns></returns>
    [HttpPost("RequestMultipleSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult RequestMultipleSMS(
        [FromQuery] int count,
        [FromBody] UserRequestDataBase request,
        [FromQuery] bool isTimeDescendingOrder = false
    )
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

        if (!ConfigDatabase.Instance.IsEnableUserAccessSMS)
        {
            // 访问短信未启用，返回403 Forbidden状态码
            return StatusCode(403, new ReturnStringData
            {
                Success = false,
                Message = "访问短信未启用！"
            });
        }

        if (!ConfigDatabase.Instance.IsEnableUserMultipleVisitsAtOnceSMS)
        {
            // 访问短信未启用，返回403 Forbidden状态码
            return StatusCode(403, new ReturnStringData
            {
                Success = false,
                Message = "单次请求访问多个短信未启用！"
            });
        }

        var data = SMSDatabase.Instance.RequestSMS(count, isTimeDescendingOrder);

        if (data == null)
            return Ok(new ReturnStringData
            {
                Success = false,
                Message = "获取失败！"
            });

        // 将data对象转换为JSON字符串
        var jsonData = JsonSerializer.Serialize(data);

        var encryptData = NetworkEncryption.Instance.Encrypt(jsonData, out var aesIv);

        return Ok(new ReturnStringData
        {
            Success = true,
            Message = "获取成功！",
            Data = encryptData,
            AesIv = aesIv
        });
    }

    /// <summary>
    /// 禁用短信
    /// </summary>
    [HttpPost("DisableSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult DisableSMS([FromBody] UserRequestStringData request)
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

        if (string.IsNullOrEmpty(request.Data))
            return BadRequest(new ReturnDataBase
            {
                Success = false,
                Message = "数据不能为空！"
            });

        if (!SMSDatabase.Instance.SetSMSIsEnable(request.Data, false))
            return Ok(new ReturnDataBase
            {
                Success = false,
                Message = "禁用失败！"
            });

        return Ok(new ReturnDataBase
        {
            Success = true,
            Message = "禁用成功！"
        });
    }

    /// <summary>
    /// 删除短信
    /// </summary>
    [HttpPost("DeleteSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult DeleteSMS([FromBody] UserRequestStringData request)
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

        if (string.IsNullOrEmpty(request.Data))
            return BadRequest(new ReturnDataBase
            {
                Success = false,
                Message = "数据不能为空！"
            });

        if (!SMSDatabase.Instance.DeleteSMS(request.Data))
            return Ok(new ReturnDataBase
            {
                Success = false,
                Message = "删除失败！"
            });

        return Ok(new ReturnDataBase
        {
            Success = true,
            Message = "删除成功！"
        });
    }

    [HttpGet("Test")]
    public string Test()
    {
        var data = SMSDatabase.Instance.RequestSMS(8000, false, new SMSFilterDatabaseData("aaa")
        {
            EnabledStatus = EnumSMSFilterEnabledStatus.Disabled
        });

        var str = "";
        
        if (data != null) str = data.Aggregate("", (current, item) => current + item.Content + "\n");
        
        return str;
    }
}