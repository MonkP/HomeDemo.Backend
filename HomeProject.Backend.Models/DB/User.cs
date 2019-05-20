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
    }
}
