using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HomeProject.Backend.Models.DB;
using HomeProject.Backend.Models.Dto;

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
    }
}
