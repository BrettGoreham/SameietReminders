using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SameietReminders
{
    public class EmailSender
    {
        public async static Task SendEmailToAddresses(List<string> emailAddresses)
        {
            Console.WriteLine(string.Join(", ", emailAddresses));

            var fromAddress = new MailAddress("inndalsveien38sameiet@gmail.com", "Sameiet Inndalsveien 38");

            var message = new MailMessage();
            message.From = fromAddress;
            

            foreach(var email in emailAddresses)
            {
                message.To.Add(new MailAddress(email));
            }
            message.Body = "It is your week to take out the garbage!";
            message.Subject = "Garbage Reminder!";


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, Environment.GetEnvironmentVariable("email_key"))
            };
           
            await smtp.SendMailAsync(message);
            
        }
    }
}
