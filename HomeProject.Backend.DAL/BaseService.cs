using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace HomeProject.Backend.DAL
{
    public class BaseService<T> where T : class, new()
    {
        public DbContext db = EFContextFactory.GetCurrentDbContext();
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
            return db.Set<T>().Count(whereExp) != 0;
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
    }
}
