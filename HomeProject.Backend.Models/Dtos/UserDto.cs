using System;
using System.Collections.Generic;
using System.Text;
using HomeProject.Backend.Models.DB;

namespace HomeProject.Backend.Models.Dto
{
    [Serializable]
    public class UserDto
    {
        public string Id { get; set; }
        public string UserCode { get; set; }
        public string DisplayName { get; set; }
    }
}
