using System;
using System.Collections.Generic;
using System.Text;

namespace HomeProject.Backend.Common.Caching
{
    public class CacheKeys
    {
        public static string UserActivateToken(string userCode)
        {
            return string.Format("UserActivateToken_", userCode);
        }
    }
}
