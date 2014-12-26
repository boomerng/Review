using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace MyE
{
    public class MyEmail
    {
        const string credentialId = "myreviewserver@gmail.com";
        const string credentialPw = "qwertygmail";
        const string host = "smtp.gmail.com";
        const int port = 465;

        public static void ChangePasswordEmail(string toEmail, string changePasswordLink, int expiryDuration, string fullName)
        {
            string subject = "Review: Change Password";
            string body = "Hi{0},<br/>An change password request is made in Review. Please \"Change Password\" below to change your password." + 
                           "Please keep in mind that this request will expire in {1} day. <br/>" + 
                            "<a href=\"{2}\">Change Password</a><br/><br/>Cheers,<br/><br/>Review Admin";
            fullName = fullName.Length > 0 ? " " + fullName : fullName;
            body = string.Format(body, fullName, expiryDuration.ToString(), changePasswordLink);

            MailMessage msg = new MailMessage(credentialId, toEmail, subject, body);
            msg.IsBodyHtml = true;
            SendEmail(msg);
        }

        public static void SendEmail(MailMessage msg)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential("myreviewserver@gmail.com", "qwertygmail");
            smtp.EnableSsl = true;
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 25;
            try
            {
                smtp.Send(msg);
            }
            catch (Exception e)
            {
                //TODO: build a log system.
            }
        }
    }
}
