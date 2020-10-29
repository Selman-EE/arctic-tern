using Application.Model;
using Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MailCarrierController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        public MailCarrierController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        [ActionName("send-mail")]
        public IActionResult SendMail([FromBody] Message message)
        {
            _emailSender.SendEmail(message);
            return Ok(new { Status = true, Message = "Sent" });
        }
        [HttpPost]
        [ActionName("send-mailasync")]
        public async Task<IActionResult> SendMailAsync()
        {
            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
            var message = new Message(new string[] { "testmail@info.com" }, "Test email", "This is the content from our email.", files);
            await _emailSender.SendEmailAsync(message);
            return Ok(new { Status = true, Message = "Sent" });
        }
    }
}
