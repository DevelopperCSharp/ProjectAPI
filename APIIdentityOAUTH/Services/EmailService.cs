using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using SendGrid;

namespace APIIdentityOAUTH.Services
{

    //l'interface "IIdentityMessageService", cette interface peut être utilisée pour configurer votre service
    //pour envoyer des courriels ou des messages SMS, tout ce que vous devez faire est 
    //de mettre en œuvre votre service de messagerie ou SMS dans la méthode "SendAsync "
    public class EmailService:IIdentityMessageService
    {
        public  async Task SendAsync(IdentityMessage message)
        {
            await configSendGridAsync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridAsync(IdentityMessage message)
        {
            var myMessage = new SendGridMessage();

            myMessage.AddTo(message.Destination);
            myMessage.From = new System.Net.Mail.MailAddress("layal_barakat@orange.fr", "layal barakat");
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = message.Body;

            var credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailService:Account"],
                ConfigurationManager.AppSettings["emailService:Password"]);

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            await transportWeb.DeliverAsync(myMessage);
        }
    }
}