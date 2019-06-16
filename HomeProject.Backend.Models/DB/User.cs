using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HomeProject.Backend.Models.DB
{
    public class User
    {
        [Key]
        [MaxLength(64)]
        public string Id { get; set; }
        /// <summary>
        /// 应采用邮箱格式保存
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string UserCode { get; set; }
        [MaxLength(64)]
        public string DisplayName { get; set; }
        [Required]
        [MaxLength(32)]
        public string PasswordMD5 { get; set; }
        [Required]
        [MaxLength(64)]
        public string PasswordSHA1 { get; set; }
        /// <summary>
        /// 是否已锁定
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// 是否已激活
        /// </summary>
        public bool Activated { get; set; }
    }
}
