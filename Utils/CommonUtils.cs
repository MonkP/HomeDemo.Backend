using System;
using System.Text;
using System.Security.Cryptography;
namespace coreMVCproject1.Utils
{
    /// <summary>
    /// 静态工具类
    /// </summary>
    public static class CommonUtils
    {
        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="code">Base64编码的密文</param>
        /// <returns>解码后的原文</returns>
        public static string Base64Decode(string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Base64Encode(string text)
        {
            string code = "";
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            try
            {
                code = Convert.ToBase64String(bytes);
            }
            catch
            {
                code = text;
            }
            return code;
        }
    }
}