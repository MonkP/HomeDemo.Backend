using System;
using System.Collections.Generic;
using System.Text;

namespace HomeProject.Backend.Common.ConfigModels
{
    public class EMailSettingsModel
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
        public string SenderPassword { get; set; }
    }
}
