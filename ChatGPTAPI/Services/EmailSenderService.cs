using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace ChatGPTAPI.Services
{
    public class EmailSenderService
    {
    private readonly IConfiguration _configuration;
    public EmailSenderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
        public async Task sendEmailAsync(string userEmail, string userName, string body)
        {
            var fromAddress = new MailAddress("saientific.lnu@gmail.com", "scAIentific");
            var toAddress = new MailAddress(userEmail, userName);
            string fromPassword = _configuration["AppPassword"];

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Chat history",
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}