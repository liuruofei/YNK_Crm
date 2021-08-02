using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Common
{
    /// <summary>
    /// 邮箱帮助类
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email_name">发件人名称</param>
        /// <param name="email_username">发件人账号</param>
        /// <param name="email_password">发件人密码</param>
        /// <param name="email_host">发件人地址</param>
        /// <param name="email_smtp">发件人SMTP地址</param>
        /// <param name="email_port">发件人SMTP端口</param>
        /// <param name="email_ssl">是否开启SSL</param>
        /// <param name="contents">邮件内容（与contentsHtml二选一）</param>
        /// <param name="contentsHtml">邮件内容（html格式）（与contents二选一）</param>
        /// <param name="collectHost">收件人地址</param>
        /// <param name="accessoryList">附件集合</param>
        public static bool Send(string email_name, string email_username, string email_password, string email_host, string email_smtp, string email_port, bool email_ssl, string title, string contents, string contentsHtml, string collectHost, List<MimePart> accessoryList = null)
        {
            try
            {
                //发件人名称
                var name = email_name;
                name = "药明康德";
                //发件人地址
                var host = email_host;
                //发件人账号
                var userName = email_username;
                //发件人密码
                var password = email_password;
                //发件人SMTP服务地址
                var smtp = email_smtp;
                //发件人SMTP服务端口
                var smtpPort = email_port;
                //是否使用ssl加密
                var UseSSL = email_ssl;

                SendEmail(title, contents, contentsHtml, name, host, userName, password, smtp, Convert.ToInt32(smtpPort), collectHost, UseSSL, accessoryList);
                return true;
            }
            catch (Exception ex)
            {
                //log.Error("发送邮件失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="subject">邮件标题</param>
        /// <param name="bodyText">邮件内容</param>
        /// <param name="bodyHtml">邮件内容(html格式)</param>
        /// <param name="name">发件人名称</param>
        /// <param name="host">发件人地址</param>
        /// <param name="username">发件人账号</param>
        /// <param name="password">发件人密码</param>
        /// <param name="smtp">发件人smtp服务</param>
        /// <param name="smtpport">发件人smtp服务端口</param>
        /// <param name="toemail">收件人地址</param>
        public static bool SendEmail(string subject, string bodyText, string bodyHtml, string name, string host, string username, string password, string smtp, int smtpport, string toemail, bool useSSL, List<MimePart> accessoryList = null)
        {
            try
            {
                var message = new MimeMessage();
                //发件人配置
                message.From.Add(new MailboxAddress(name, host));
                //收件人配置
                message.To.Add(new MailboxAddress(toemail));
                //邮件标题
                message.Subject = subject;
                //邮件内容
                //message.Body = new TextPart("plain") { Text = body };
                var alternative = new Multipart("alternative");
                if (!string.IsNullOrWhiteSpace(bodyText))
                {
                    alternative.Add(new TextPart("plain")
                    {
                        Text = bodyText
                    });
                }

                if (!string.IsNullOrWhiteSpace(bodyHtml))
                {
                    alternative.Add(new TextPart("html")
                    {
                        Text = bodyHtml
                    });
                }
                var multipart = new Multipart("mixed");
                multipart.Add(alternative);
                if (accessoryList != null)
                {
                    accessoryList?.ForEach(f =>
                    {
                        multipart.Add(f);
                    });

                }
                message.Body = multipart;
                //HTML邮件内容
                //var bodyBuilder = new BodyBuilder();
                //bodyBuilder.HtmlBody = @"<b>This is bold and this is <i>italic</i></b>";
                //message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(smtp, smtpport, useSSL);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(username, password);

                    client.Send(message);
                    client.Disconnect(true);
                    Console.WriteLine($"给{toemail}发送邮件成功");
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }
    }
}
