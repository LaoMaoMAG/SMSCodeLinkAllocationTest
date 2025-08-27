using APIModes;
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
    public IActionResult AddSMS([FromBody] AdminRequestStringData requestString)
    {
        if (!VerifyAdminAccount(requestString)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        if (requestString.Data == null) 
            return BadRequest(new ReturnDataBase { Success = false, Message = "Content 不能为空！" });
        
        var isSuccess = SMSDatabaseManager.Instance.AddSMS(requestString.Data);
        
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
    public IActionResult AddMultipleSMS([FromBody] AdminRequestStringListData requestStringList)
    {
        if (!VerifyAdminAccount(requestStringList)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        if (requestStringList.Data == null) 
            return BadRequest(new ReturnDataBase { Success = false, Message = "Content 不能为空！" });
        
        var isSuccess = SMSDatabaseManager.Instance.AddSMS(requestStringList.Data);
        
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
        
        Console.WriteLine(pageIndex + " " + pageSize);
        
        var smsList = SMSDatabaseManager.Instance.GetSMSPageData(pageIndex, pageSize);
        var smsTotal = SMSDatabaseManager.Instance.GetSMSTotal();
        
        var totalPages = smsTotal / pageSize;
        if (totalPages == 0 && smsTotal != 0) totalPages = 1;
        
        var maxNumber = pageSize * pageIndex;
        var miniNumber = maxNumber - pageSize + 1;
        
        
        if (smsList == null || smsList.Count == 0)
        {
            return Ok(new AdminReturnSMSPageData
            {
                SMSList = smsList,
                SMSTotal = smsTotal,
                TotalPages = totalPages,
                Success = false,
                Message = "没有数据！",
                MiniNumber = miniNumber,
                MaxNumber = maxNumber
            });   
        }
        
        return Ok(new AdminReturnSMSPageData
        {
            SMSList = smsList,
            SMSTotal = smsTotal,
            TotalPages = totalPages,
            Success = true,
            Message = "获取数据成功！",
            MiniNumber = miniNumber,
            MaxNumber = maxNumber
        });
    }
    
    /// <summary>
    /// 删除所有短信
    /// </summary>
    [HttpPost("DeleteAllSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult DeleteAllSMS([FromBody] AdminRequestDataBase request)
    {
        if (!VerifyAdminAccount(request)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        var isSuccess = SMSDatabaseManager.Instance.DeleteAllSMS();
        
        return Ok(isSuccess
            ? new ReturnDataBase { Success = true, Message = "删除数据成功！" }
            : new ReturnDataBase { Success = false, Message = "删除数据失败！" });
    }
    
    /// <summary>
    /// 获取服务器启动时间
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("GetServerStartTime")]
    public IActionResult GetServerStartTime ([FromBody] AdminRequestDataBase request)
    {
        if (!VerifyAdminAccount(request)) 
            return Unauthorized(new ReturnLongData { Success = false, Message = "管理员认证失败！" });
        
        
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var time = (long)(Program.StartTime.ToUniversalTime() - epoch).TotalMilliseconds;
        
        Console.WriteLine(time);
        
        return Ok(new ReturnLongData
        {
            Success = true,
            Message = "获取数据成功！",
            Data = time
        });
    }
    
    /// <summary>
    /// 获取统计数据
    /// </summary>
    [HttpPost("GetStatisticalData")]
    public IActionResult GetStatisticalData ([FromBody] AdminRequestDataBase request)
    {
        if (!VerifyAdminAccount(request)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        return Ok (new AdminReturnStatisticalData
        {
            Success = true,
            Message = "获取数据成功！",
            UserApiRequestsNumber = ConfigDatabase.Instance.UserApiAccessCount,
            UserApiRequestsSuccessfulSMSTotal = SMSDatabaseManager.Instance.GetAccessSMSTotal(),
            SMSTotal = SMSDatabaseManager.Instance.GetSMSTotal(),
            DisableSMSQuantity = SMSDatabaseManager.Instance.GetEnableSMSCount()
        });
    }

    /// <summary>
    /// 获取短信用户设置
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("GetUserSMSUserSettings")]
    // ReSharper disable once InconsistentNaming
    public IActionResult GetUserSMSUserSettings([FromBody] AdminRequestDataBase request)
    {
        if (!VerifyAdminAccount(request)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        return Ok(new AdminReturnUserSMSUserSettingsData
        {
            Success = true,
            Message = "获取成功！",
            Data = new UserSMSUserSettingsData
            {
                IsEnableUserAccessSMS = ConfigDatabase.Instance.IsEnableUserAccessSMS,
                IsEnableUserMultipleVisitsAtOnceSMS = ConfigDatabase.Instance.IsEnableUserMultipleVisitsAtOnceSMS,
                UserAccessKey = ConfigDatabase.Instance.UserAccessKey,
                UserAccessEncryptionKey = ConfigDatabase.Instance.UserAccessEncryptionKey,
            },
        });
    }
    
    /// <summary>
    /// 设置短信用户设置
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("SetUserSMSUserSettings")]
    // ReSharper disable once InconsistentNaming
    public IActionResult SetUserSMSUserSettings([FromBody] AdminRequestUserSMSUserSettingsData request)
    {
        if (!VerifyAdminAccount(request)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });

        if (request.Data == null) 
            return BadRequest(new ReturnDataBase { Success = false, Message = "Data 不能为空！" });
        
        ConfigDatabase.Instance.IsEnableUserAccessSMS = request.Data.IsEnableUserAccessSMS;
        ConfigDatabase.Instance.IsEnableUserMultipleVisitsAtOnceSMS = request.Data.IsEnableUserMultipleVisitsAtOnceSMS;
        ConfigDatabase.Instance.UserAccessKey = request.Data.UserAccessKey;
        ConfigDatabase.Instance.UserAccessEncryptionKey = request.Data.UserAccessEncryptionKey;
        
        return Ok(new ReturnDataBase
        {
            Success = true,
            Message = "设置成功！",
        });
    }

    /// <summary>
    /// 获取所有短信内容
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("GetAllSMSContent")]
    // ReSharper disable once InconsistentNaming
    public IActionResult GetAllSMSContent([FromBody] AdminRequestDataBase request)
    {
        if (!VerifyAdminAccount(request)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        var data = SMSDatabaseManager.Instance.GetAllSMSContent();
        
        return data == null
            ? Ok(new ReturnStringListData { Success = false, Message = "获取数据失败！" })
            : Ok(new ReturnStringListData
            {
                Success = true,
                Message = "获取成功！",
                Data = data
            });
    }

    /// <summary>
    /// 禁用短信
    /// </summary>
    /// <returns></returns>
    [HttpPost("DisableSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult DisableSMS([FromBody] AdminRequestStringListData request)
    {
        if (!VerifyAdminAccount(request)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        if (request.Data == null) 
            return BadRequest(new ReturnDataBase { Success = false, Message = "数据不能为空！" });
        
        var isSuccess = SMSDatabaseManager.Instance.DisableSMS(request.Data);

        if (!isSuccess)
        {
            return Ok(new ReturnDataBase
            {
                Success = false,
                Message = "禁用失败！",
            });
        }
        
        return Ok(new ReturnDataBase
        {
            Success = true,
            Message = "禁用成功！",
        });
    }
    
    /// <summary>
    /// 删除短信
    /// </summary>
    /// <returns></returns>
    [HttpPost("DeleteSMS")]
    // ReSharper disable once InconsistentNaming
    public IActionResult DeleteSMS([FromBody] AdminRequestStringListData request)
    {
        if (!VerifyAdminAccount(request)) 
            return Unauthorized(new ReturnDataBase { Success = false, Message = "管理员认证失败！" });
        
        if (request.Data == null) 
            return BadRequest(new ReturnDataBase { Success = false, Message = "Data 不能为空！" });
        
        var isSuccess = SMSDatabaseManager.Instance.DeleteSMS(request.Data);
        
        return Ok(isSuccess
            ? new ReturnDataBase { Success = true, Message = "删除数据成功！" }
            : new ReturnDataBase { Success = false, Message = "删除数据失败！" });
    }
}