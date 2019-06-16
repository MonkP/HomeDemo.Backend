using Microsoft.AspNetCore.Mvc;
using HomeProject.Backend.Common;
using HomeProject.Backend.Common.ConfigModels;
using HomeProject.Backend.Models.Dto;
using Microsoft.Extensions.Options;
using HomeProject.Backend.DAL;
using Microsoft.Extensions.Caching.Memory;
using HomeProject.Backend.Common.Caching;
using System;

namespace HomeProject.Backend.Server.Controllers
{
    public class LoginController : BaseController
    {
        UserService service = new UserService();
        /// <summary>
        /// 构造函数，向基类透传配置对象
        /// 同时声明使用缓存，获取依赖注入
        /// </summary>
        /// <param name="options"></param>
        public LoginController(IOptions<ConfigModel> options, IMemoryCache memoryCache) : base(options)
        {
            _cache = memoryCache;
        }
        /// <summary>
        /// 登录校验方法
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码的密文</param>
        /// <returns></returns>
        public JsonResult ValidateLogin(string userName, string pwd)
        {
            //密码先解码成原始密文
            pwd = StringUtils.Base64Decode(pwd);

            var matchedUser = service.GetMatchedUser(userName, pwd);
            if (matchedUser != null)
            {
                //初始化Session
                SetObjectToSession("UserInfo", matchedUser);
                //返回登录成功结果
                return Json(new AjaxMessageDto
                {
                    success = true,
                    msg = "登录成功"
                });
            }
            else
            {
                return Json(new AjaxMessageDto
                {
                    success = false,
                    msg = "用户名或密码错误"
                });
            }
        }
        public JsonResult SignUp(string userInfo, string pwdMD5, string pwdSHA1)
        {
            UserDto userDto = fromJson<UserDto>(userInfo);
            //检查用户名占用
            var existUserCode = service.Exist(p => p.UserCode == userDto.UserCode);
            if (!existUserCode)
            {
                var saveResult = service.UserSignUp(userDto, pwdMD5, pwdSHA1);
                var serverUrl = HttpContext.Request.Host.ToString();
                var tokenInCache = Guid.NewGuid().ToString();
                var cacheKey = CacheKeys.UserActivateToken(userDto.UserCode);
                setCache<string>(cacheKey, tokenInCache);
                var mailResult = service.SendActivationEmail(userDto, "", serverUrl, config.EMailSettings);
                if (string.IsNullOrEmpty(mailResult))
                {
                    return Json(new AjaxMessageDto
                    {
                        success = true,
                        msg = "请检查激活邮件"
                    });
                }
                else
                {
                    return Json(new AjaxMessageDto
                    {
                        success = false,
                        msg = "激活邮件发送失败！"
                    });
                }
            }
            else {
                return Json(new AjaxMessageDto
                {
                    success = false,
                    msg = "该用户名已注册！"
                });
            }
        }
    }
}