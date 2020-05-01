using Quartz;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace tests
{
    public class JobEmail : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (var message = new MailMessage("user@gmail.com", "user@live.co.uk"))
            {
                message.Subject = "Test";
                message.Body = "Test at " + DateTime.Now;
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("user@gmail.com", "password")
                })
                {
                    client.Send(message);
                }
            }

            return Task.FromResult<bool>(true);
        }
    }
}