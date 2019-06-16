using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using HomeProject.Backend.Common;
using Microsoft.Extensions.Options;
using HomeProject.Backend.Common.ConfigModels;
using Microsoft.Extensions.Caching.Memory;

namespace HomeProject.Backend.Server.Controllers
{
    public class BaseController: Controller
    {
        protected ConfigModel config;
        protected IMemoryCache _cache;
        public BaseController(IOptions<ConfigModel> options)
        {
            this.config = options.Value;
        }
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
        #region JSON辅助方法
        protected T fromJson<T>(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch
            {
                return default(T);
            }
        }
        protected string toJson(Object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        #endregion
        #region 缓存辅助方法
        protected T setCache<T>(string key,T value)
        {
            return _cache.Set<T>(key, value);
        }
        protected T getCache<T>(string key) where T:class
        {
            object value;
            bool exist = _cache.TryGetValue(key, out value);
            if (exist)
            {
                return value as T;
            }
            else
            {
                return default(T);
            }
        }
        #endregion
    }
}
