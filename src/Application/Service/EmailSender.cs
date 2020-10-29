using Application.Model;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }
        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        protected internal MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<h4 style='color:red;'>{message.Content}</h4>"
            };
            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                var fileName = string.Empty;
                foreach (var attachment in message.Attachments)
                {
                    fileBytes = GetByteArrayFromImage(attachment);
                    fileName = GetUniqueFileName(attachment.FileName);
                    bodyBuilder.Attachments.Add(fileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }


        protected internal void Send(MimeMessage mailMessage)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable); // configure to your SMTP server
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                //add log error or throw ex or both
                throw;
            }
            finally
            {
                client.Disconnect(true);
            }
        }
        protected internal async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable); // configure to your SMTP server
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                await client.SendAsync(mailMessage);
            }
            catch (Exception ex)
            {
                //add log error or throw ex or both
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        private static byte[] GetByteArrayFromImage(IFormFile file)
        {
            using var target = new MemoryStream();
            file.CopyTo(target);
            return target.ToArray();
        }
        private static string GetUniqueFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;
            //
            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid().ToString().Substring(0, 4)}{Path.GetExtension(fileName)}";
            return fileName;
        }
    }
}
