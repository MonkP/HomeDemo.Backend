using System;

using System.Text;
namespace HomeDemo.Backend.Common
{
    public class StringUtils
    {
        /// <summary>
        /// Base64解码方法
        /// </summary>
        /// <param name="cypher">加密原文</param>
        /// <returns>密文</returns>
        public static string Base64Decode(string cypher)
        {
            byte[] bytes = Convert.FromBase64String(cypher);
            try
            {
                var toReturn = Encoding.UTF8.GetString(bytes);
                return toReturn;
            }
            catch
            {
                return cypher;
            }
        }
        /// <summary>
        /// Base64编码方法
        /// </summary>
        /// <param name="origin">原始明文</param>
        /// <returns></returns>
        public static string Base64Encode(string origin)
        {
            var bytes = Encoding.UTF8.GetBytes(origin);
            try
            {
                var encode = Convert.ToBase64String(bytes);
                return encode;
            }
            catch
            {
                return origin;
            }
        }
    }
}
