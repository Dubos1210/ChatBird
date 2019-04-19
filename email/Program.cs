using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Net.Mail;

namespace ChatBirdBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            string email = Console.ReadLine();
            if (email == "") email = "dubos1210@yandex.ru";

            string path = "";

            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"settings");
                file.ReadLine();
                path = file.ReadLine();
                if (path == null) return;
                file.Close();
            
                MailMessage mail = new MailMessage("no-reply.dubos@yandex.ru", email);
                mail.Subject = "ChatBox Backup";
                mail.Body = System.DateTime.Now.ToString() + " " + System.Net.Dns.GetHostName();
                mail.Attachments.Add(new Attachment("settings"));
                mail.Attachments.Add(new Attachment(path));
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.yandex.ru";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("no-reply.dubos", "dubos2017");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
