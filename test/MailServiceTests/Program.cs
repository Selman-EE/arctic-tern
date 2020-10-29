using System;
using System.Net.Mail;
using System.Net;

namespace MailServiceTests
{
    static class Program
    {
        static void Main(string[] args)
        {
            //https://mailtrap.io/
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("219c0703259cd9", "47fb1f217c53e6"),
                EnableSsl = true
            };
            //
            client.Send("from@example.com", "to@example.com", "Hello world", "testbody");
            Console.WriteLine("Sent");
            Console.ReadLine();
        }
    }
}
