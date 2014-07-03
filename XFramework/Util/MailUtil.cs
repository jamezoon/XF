using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XFramework.Util
{
    public class MailUtil
    {
        public static MailUtil Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly MailUtil instance = new MailUtil();
        }

        public void SendEmail(string subject, string body, string[] to, bool EnableSsl = false)
        {
            var smtpClient = new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["MailHost"],
                EnableSsl = EnableSsl
            };

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            foreach (var item in to)
            {
                mailMessage.To.Add(item);
            }

            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MailLoginName"],
                Safe.Aes.Decrypt(ConfigurationManager.AppSettings["MailPassword"], "12345678"));

            smtpClient.Send(mailMessage);
        }
    }
}
