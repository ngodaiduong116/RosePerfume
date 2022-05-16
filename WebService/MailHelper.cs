﻿using System.Net;
using System.Configuration;
using System.Net.Mail;
using System;

namespace WebService
{
    public class MailHelper
    {
        public void SendMail(string toEmail, string subject, string content)
        {
            var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
            var fromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"].ToString();
            var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
            var sMTPHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
            var sMTPPost = ConfigurationManager.AppSettings["SMTPPost"].ToString();
            //var toEmailAddress = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

            bool enabledSsl = bool.Parse(ConfigurationManager.AppSettings["EnabledSSL"].ToString());

            string body = content;
            MailMessage message = new MailMessage(new MailAddress(fromEmailAddress, fromEmailDisplayName), new MailAddress(toEmail));
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            var client = new SmtpClient();
            client.Credentials = new NetworkCredential(fromEmailAddress, fromEmailPassword);
            client.Host = sMTPHost;
            client.EnableSsl = enabledSsl;
            client.Port = !string.IsNullOrEmpty(sMTPHost) ? Convert.ToInt32(sMTPPost) : 0;
            client.Send(message);
        }
    }
}