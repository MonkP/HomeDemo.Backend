using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Web;
using System.Net;
using HomeProject.Backend.Common.ConfigModels;

namespace HomeProject.Backend.Common
{
    public class MailService
    {
        public static string SendEMail(string content,string reciver,string subject, EMailSettingsModel senderSettings)
        {
            MailMessage mailmessage = new MailMessage(new MailAddress(senderSettings.SenderAddress,senderSettings.SenderName), new MailAddress(reciver));
            mailmessage.Subject = subject;
            mailmessage.Body = content;
            mailmessage.BodyEncoding = Encoding.Unicode;

            //from email，to email，主题，邮件内容
            mailmessage.Priority = MailPriority.Normal; //邮件优先级
            SmtpClient smtpClient = new SmtpClient(senderSettings.SmtpServer, senderSettings.SmtpPort); //smtp地址以及端口号
            smtpClient.Credentials = new NetworkCredential(senderSettings.SenderAddress, senderSettings.SenderPassword);//smtp用户名密码
            smtpClient.EnableSsl = true; //启用ssl
            try
            {
                smtpClient.Send(mailmessage); //发送邮件
                return "";
            }
            catch(Exception ex)
            {
                return ex.Message + "\n" + ex.StackTrace;
            }
        }
    }
}
