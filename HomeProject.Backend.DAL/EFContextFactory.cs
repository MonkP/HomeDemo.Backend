using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using HomeProject.Backend.Models.DB;
using System.Collections.Concurrent;

namespace HomeProject.Backend.DAL
{
    public class EFContextFactory
    {
        /// <summary>
        /// 提供跨线程的实例访问
        /// </summary>
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();
        //private static Hashtable cache = Hashtable.Synchronized(new Hashtable());//缓存
        /// <summary>
        /// 帮我们返回当前线程内的数据库上下文，如果当前线程内没有上下文，那么创建一个上下文，并保证
        /// 上下文是实例在线程内部唯一
        /// 在EF4.0以前使用ObjectsContext对象
        /// </summary>
        /// <returns></returns>
        public static DbContext GetCurrentDbContext()
        {
            //当第二次执行的时候直接取出线程嘈里面的对象
            //使用静态state对象作为线程内部唯一的独用的数据槽(一块内存空间)
            //数据存储在线程栈中
            //线程内共享一个单例
            DbContext dbcontext = GetData("DbContext") as DbContext;

            //判断线程里面是否有数据
            if (dbcontext == null)  //线程的数据槽里面没有次上下文
            {
                dbcontext = new HomeDbContext();

                //存储指定对象
                SetData("DbContext", dbcontext);
            }
            return dbcontext;
        }
        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData(string name, object data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static object GetData(string name) =>
            state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;
    }
}
