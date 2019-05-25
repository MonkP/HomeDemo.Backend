using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HomeProject.Backend.Common;

namespace HomeProject.Backend.Server.Controllers
{
    public class BaseController: Controller
    {
        /// <summary>
        /// 将对象加入Session
        /// </summary>
        /// <param name="key">Session KEY</param>
        /// <param name="obj">要加入Session的对象</param>
        protected virtual void SetObjectToSession<T>(string key,T obj)
        {
            var objBytes = ObjectUtils.ObjectToBytes(obj);
            HttpContext.Session.Set(key, objBytes);
        }
        /// <summary>
        /// 从Session获取对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="key">Session KEY</param>
        /// <returns></returns>
        protected virtual T GetObjectFromSession<T>(string key) where T:class
        {
            var objBytes = HttpContext.Session.Get(key);
            if(objBytes!=null && objBytes.Length!=0)
            {
                try
                {
                    T obj = ObjectUtils.BytesToObject(objBytes) as T;
                    return obj;
                }
                catch (    System.Runtime.Serialization.SerializationException)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
