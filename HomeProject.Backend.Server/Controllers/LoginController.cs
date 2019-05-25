using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeProject.Backend.Common;
using Microsoft.AspNetCore.Http;
using HomeProject.Backend.Models.Dto;
using HomeProject.Backend.DAL;

namespace HomeProject.Backend.Server.Controllers
{
    public class LoginController : BaseController
    {
        UserService service = new UserService();
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
    }
}