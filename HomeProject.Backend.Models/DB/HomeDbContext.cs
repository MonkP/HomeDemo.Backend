using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HomeProject.Backend.Models.DB
{
    public class HomeDbContext: DbContext
    {
        /// <summary>
        /// 初始化配置，指定SQLite文件
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Home.db");
        }
        /// <summary>
        /// 用户表
        /// </summary>
        public DbSet<User> User { get; set; }
    }
}
