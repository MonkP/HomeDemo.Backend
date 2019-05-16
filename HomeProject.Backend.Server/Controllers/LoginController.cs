using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using coreMVCproject1.Models;
using coreMVCproject1.Utils;
using Microsoft.AspNetCore.Http;
using coreMVCproject1.Models.Dto;

namespace coreMVCproject1.Controllers
{
    public class LoginController : Controller
    {
        /// <summary>
        /// 登录校验方法
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码的密文</param>
        /// <returns></returns>
        public JsonResult ValidateLogin(string userName, string pwd)
        {
            //密码先解码成原始密文
            pwd = CommonUtils.Base64Decode(pwd);
            // TODO 
            //假装匹配密码

            //初始化Session
            HttpContext.Session.SetString("userName", userName);

            //返回登录成功结果
            return Json(new AjaxMessageDto
            {
                success = true,
                msg = "登录成功"
            });
        }
    }
}