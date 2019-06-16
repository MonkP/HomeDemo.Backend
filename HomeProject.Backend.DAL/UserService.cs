using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HomeProject.Backend.Models.DB;
using HomeProject.Backend.Models.Dto;
using HomeProject.Backend.Common;
using HomeProject.Backend.Common.ConfigModels;

namespace HomeProject.Backend.DAL
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserService:BaseService<User>
    {
        public UserDto GetMatchedUser(string userCode, string pwdMD5)
        {
            var user = Find(p => p.UserCode == userCode.ToUpper() && p.PasswordMD5 == pwdMD5.ToLower());
            if (user != null)
            {
                return new UserDto
                {
                    UserCode = user.UserCode,
                    Id = user.Id,
                    DisplayName = user.DisplayName
                };
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 用户注册，同时发送激活邮件
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="pwdMD5"></param>
        /// <param name="pwdSHA1"></param>
        /// <returns></returns>
        public string UserSignUp(UserDto dto, string pwdMD5,string pwdSHA1)
        {
            var userEntity = new User
            {
                Id = Guid.NewGuid().ToString(),
                PasswordMD5 = pwdMD5,
                DisplayName = dto.DisplayName,
                PasswordSHA1 = pwdSHA1,
                UserCode = dto.UserCode,
                Locked = false,
                Activated = false
            };
            //保存到数据库
            AddEntity(userEntity);
            return userEntity.Id;
        }
        /// <summary>
        /// 发送用户注册确认邮件
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="token"></param>
        /// <param name="eMailSettings"></param>
        /// <returns></returns>
        public string SendActivationEmail(UserDto dto,string token,string serverUrl, EMailSettingsModel eMailSettings)
        {
            //拼接邮件内容
            string mailTemplate = @"
<!DOCTYPE html>
<html><head></head>
<body>
<p>{0},欢迎注册HomeProject。</p>
<p>请点击<a href='{1}'>这个链接</a>,完成注册。</p>
<p>如果您没有注册HomeProject，请忽略本邮件。</p>
</body>
</html>
";
            string linkUrlTemplate = "{0}/Login/MailActivate?userCode={1}&token={2}";
            string linkUrl = string.Format(linkUrlTemplate, serverUrl, dto.UserCode, token);
            string mailBody = string.Format(mailTemplate, dto.DisplayName, linkUrl);
            //调用邮件服务
            var toReturn = MailService.SendEMail(mailBody, dto.UserCode, "欢迎注册HomeProject,请激活您的账号", eMailSettings);
            return toReturn;
        }
    }
}
