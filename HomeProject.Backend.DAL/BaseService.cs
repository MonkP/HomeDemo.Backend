using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NLog;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Extensions;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using System.Transactions;

namespace HomeProject.Backend.DAL
{
    public class BaseService<T> where T : class, new()
    {
        public DbContext db = EFContextFactory.GetCurrentDbContext();

        private static readonly Logger Logger = LogManager.GetLogger("BaseService");
        #region 新增方法
        /// <summary>
        /// 将单一实体保存到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        public virtual T AddEntity(T entity,bool async = false)
        {
            db.Entry<T>(entity).State = EntityState.Added;
            if (!async)
            {
                db.SaveChanges();
            }
            return entity;
        }
        public virtual int AddRange(IEnumerable<T> entities,bool async = false)
        {
            int toReturn = 0;
            foreach(var entity in entities)
            {
                db.Entry<T>(entity).State = EntityState.Added;
                toReturn++;
            }
            if (!async)
            {
                db.SaveChanges();
            }
            return toReturn;
        }
        #endregion

        #region 查询方法
        /// <summary>
        /// 单体查询
        /// </summary>
        /// <param name="whereExp">查询条件表达式</param>
        /// <returns></returns>
        public virtual T Find(Expression<Func<T,bool>> whereExp)
        {
            return db.Set<T>().FirstOrDefault(whereExp);
        }
        public virtual bool Exist(Expression<Func<T, bool>> whereExp)
        {
            return db.Set<T>().Any(whereExp);
        }
        public virtual int Count(Expression<Func<T, bool>> whereExp)
        {
            return db.Set<T>().Count(whereExp);
        }
        /// <summary>
        /// 普通批量查询
        /// </summary>
        /// <param name="whereExp">查询条件表达式</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetEntities(Expression<Func<T, bool>> whereExp)
        {
            return db.Set<T>().Where<T>(whereExp).AsQueryable();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="wherelambda">查询条件表达式</param>
        /// <param name="pageNumber">页码，从1开始</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="sortProperty">排序属性</param>
        /// <param name="isDesc">是否降序排列</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetEntities(Expression<Func<T, bool>> wherelambda, int pageNumber, int pageSize, string sortProperty, bool isDesc = false)
        {
            var tempEntities = GetEntities(wherelambda);
            var sortDirection = isDesc ? "DSEC" : "ASC";
            return DataSortingAndPaging<T>(tempEntities, sortProperty, sortDirection, pageNumber, pageSize);
        }

        /// <summary>
        /// 对查询结果，分页排序方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortExpression"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IQueryable<T> DataSortingAndPaging<T>(IQueryable<T> source, string sortExpression, string sortDirection, int pageNumber, int pageSize)
        {
            //先排序后分页
            IQueryable<T> query = DataSorting<T>(source, sortExpression, sortDirection);
            return DataPaging(query, pageNumber, pageSize);
        }
        /// <summary>
        /// 对查询结果，排序方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">待排序的原始数据集</param>
        /// <param name="sortProperty">排序属性名</param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public virtual IQueryable<T> DataSorting<T>(IQueryable<T> source, string sortProperty, string sortDirection)
        {
            string sortingDir = string.Empty;
            if (sortDirection.ToUpper().Trim() == "ASC")
                sortingDir = "OrderBy";
            else if (sortDirection.ToUpper().Trim() == "DESC")
                sortingDir = "OrderByDescending";
            ParameterExpression param = Expression.Parameter(typeof(T), sortProperty);
            PropertyInfo pi = typeof(T).GetProperty(sortProperty);
            Type[] types = new Type[2];
            types[0] = typeof(T);
            types[1] = pi.PropertyType;
            Expression expr = Expression.Call(typeof(Queryable), sortingDir, types, source.Expression, Expression.Lambda(Expression.Property(param, sortProperty), param));
            IQueryable<T> query = source.AsQueryable().Provider.CreateQuery<T>(expr);
            return query;
        }

        /// <summary>
        /// 对查询结果，分页方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">待分页的原始数据集</param>
        /// <param name="pageNumber">从1开始</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public virtual IQueryable<T> DataPaging<T>(IQueryable<T> source, int pageNumber, int pageSize)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
        #endregion

        #region 更新与删除方法
        /// <summary>
        /// 更新单一实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="async">是否挂起更新。如为true，则在调用SaveChange时才保存到数据库</param>
        /// <returns></returns>
        public virtual bool UpdateEntity(T entity,bool async=false)
        {
            db.Set<T>().Attach(entity);
            db.Entry<T>(entity).State = EntityState.Modified;
            return async ? true : db.SaveChanges() != 0;
        }
        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="async">是否挂起更新。如为true，则在调用SaveChange时才保存到数据库</param>
        /// <returns></returns>
        public virtual int UpdateEntities(IEnumerable<T> entities, bool async = false)
        {
            foreach (var entity in entities)
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Modified;
            }
            return async ? entities.Count() : db.SaveChanges();
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="async">是否挂起更新。如为true，则在调用SaveChange时才保存到数据库</param>
        /// <returns></returns>
        public virtual bool DeleteEntity(T entity, bool async = false)
        {
            db.Set<T>().Attach(entity);
            db.Entry<T>(entity).State = EntityState.Deleted;
            return async ? true : db.SaveChanges() != 0;
        }
        /// <summary>
        /// 删除实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="async">是否挂起更新。如为true，则在调用SaveChange时才保存到数据库</param>
        /// <returns></returns>
        public virtual int DeleteEntities(IEnumerable<T> entities, bool async = false)
        {
            foreach(var entity in entities)
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Deleted;
            }
            return async ? entities.Count(): db.SaveChanges();
        }
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="whereLambda">条件表达式</param>
        /// <param name="async">是否挂起更新。如为true，则在调用SaveChange时才保存到数据库</param>
        /// <returns></returns>
        public virtual int DeleteRange(Expression<Func<T, bool>> whereLambda,bool async = false)
        {
            var entities = db.Set<T>().Where(whereLambda);
            foreach (var entity in entities)
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Deleted;
            }
            return async ? entities.Count() : db.SaveChanges();

        }

        /// <summary>
        /// 通过id数组进行删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual int DeleteRange(IEnumerable<int> ids, bool async = false)
        {
            foreach (var item in ids)
            {
                var entity = db.Set<T>().Find(item);
                db.Entry<T>(entity).State = EntityState.Deleted;
            }
            return async ? ids.Count() : db.SaveChanges();
        }

        /// <summary>
        /// 提交修改项
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return db.SaveChanges();
            //return await db.SaveChangesAsync();
        }
        #endregion

        #region Raw SQL 操作
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="querySql">SQL语句</param>
        /// <param name="par">参数集合，可为空</param>
        /// <returns>查询集合</returns>
        public IQueryable<T> ExcuteSqlQuery(string querySql, params object[] par)
        {
            return db.Set<T>().FromSql<T>(querySql, par).AsQueryable();
        }
        /// <summary>
        /// 执行更新、删除语句
        /// </summary>
        /// <param name="sqlCommand">SQL语句</param>
        /// <param name="par">参数集合，可为空</param>
        /// <returns></returns>
        public int ExcuteSqlCommand(string sqlCommand,params object[] par)
        {
            return db.Database.ExecuteSqlCommand(sqlCommand, par);
        }
        /// <summary>
        /// 异步执行更新、删除语句
        /// </summary>
        /// <param name="sqlCommand">SQL语句</param>
        /// <param name="par">参数集合，可为空</param>
        /// <returns></returns>
        public Task<int> ExcuteSqlCommandAsync(string sqlCommand,params object[] par)
        {
            return db.Database.ExecuteSqlCommandAsync(sqlCommand, par);
        }

        #endregion

        #region 事务方法

        /// <summary>
        /// 启用事务
        /// </summary>
        /// <returns></returns>

        public TransactionScope BeginTransaction()
        {
            TransactionScope scope = new TransactionScope();
            return scope;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="scope"></param>
        public void CommitTransaction(TransactionScope scope)
        {
            scope.Complete();
        }

        /// <summary>
        /// 注销事务
        /// </summary>
        /// <param name="scope"></param>
        public void EndTransaction(TransactionScope scope)
        {
            scope.Dispose();
        }

        #endregion
        /// <summary>
        /// 字段赋值方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="val"></param>
        private void SetValue<T, K>(T obj, string propertyName, K val)
        {
            PropertyInfo propertyInfo;
            propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo != null)
            {
                Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                propertyInfo.SetValue(obj, (val == null) ? null : Convert.ChangeType(val, type), null);
            }
        }

        public void LogError(Exception ex, string errMsg = "")
        {
            var strError = new StringBuilder();
            strError.AppendFormat("Message:{0}", ex.Message);
            strError.AppendLine();
            strError.AppendFormat("Srouce:{0}", ex.Source);
            strError.AppendLine();
            strError.AppendFormat("StackTrace:{0}", ex.StackTrace);
            strError.AppendLine();
            strError.AppendFormat("Data:{0}", ex.Data);
            strError.AppendLine();
            if (!string.IsNullOrEmpty(errMsg))
            {
                strError.AppendFormat("输出消息:{0}", errMsg);
                strError.AppendLine();
            }

            strError.AppendLine("=============================================================");
            Logger.Error(strError.ToString());
        }
    }
}
