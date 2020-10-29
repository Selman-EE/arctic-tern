using Microsoft.AspNetCore.Http;
using MimeKit;
using System.Collections.Generic;


namespace Application.Model
{
    public class Message
    {
        public ICollection<MailboxAddress> To { get; set; } = new HashSet<MailboxAddress>();
        public string Subject { get; set; }
        public string Content { get; set; }
        public IFormFileCollection Attachments { get; set; }
        public Message(ICollection<string> to, string subject, string content, IFormFileCollection attachments = null)
        {
            foreach (var item in to)
                To.Add(new MailboxAddress(item));

            Subject = subject;
            Content = content;
            Attachments = attachments;
        }
    }
}
