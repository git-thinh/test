using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gemail
{
    // https://stackoverflow.com/questions/32260/sending-email-in-net-through-gmail


    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json
        //static string[] Scopes = { GmailService.Scope.GmailReadonly };
        //static string[] Scopes = { GmailService.Scope.GmailModify };
        //static string[] Scopes = { GmailService.Scope.GmailSend };
        static string[] Scopes = Scopes = new string[] {
            "https://mail.google.com",
            GmailService.Scope.GmailCompose,
            GmailService.Scope.GmailInsert,
            GmailService.Scope.GmailLabels,
            GmailService.Scope.GmailModify,
            GmailService.Scope.GmailReadonly,
            GmailService.Scope.MailGoogleCom,
            "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile"
        };

        static string ApplicationName = "Gmail API .NET Quickstart";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //====================================================================================
            //[1] GET LABELS OF EMAIL
            #region
            // Define parameters of request.
            //UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");
            //// List labels.
            //IList<Label> labels = request.Execute().Labels;
            //Console.WriteLine("Labels:");
            //if (labels != null && labels.Count > 0)
            //{
            //    foreach (var labelItem in labels)
            //    {
            //        Console.WriteLine("{0}", labelItem.Name);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No labels found.");
            //}
            ////Console.Read();
            #endregion

            //====================================================================================
            //[2] GET TOP 100 EMAILS
            #region
            //var inboxlistRequest = service.Users.Messages.List("me");
            //inboxlistRequest.LabelIds = "INBOX";
            //inboxlistRequest.IncludeSpamTrash = false;
            ////get our emails   
            //var emailListResponse = inboxlistRequest.Execute();
            //if (emailListResponse != null && emailListResponse.Messages != null)
            //{
            //    //loop through each email and get what fields you want...   
            //    foreach (var email in emailListResponse.Messages)
            //    {
            //        var emailInfoRequest = service.Users.Messages.Get("me", email.Id);
            //        var emailInfoResponse = emailInfoRequest.Execute();
            //        if (emailInfoResponse != null)
            //        {
            //            String from = "";
            //            String date = "";
            //            String subject = "";
            //            //loop through the headers to get from,date,subject, body  
            //            foreach (var mParts in emailInfoResponse.Payload.Headers)
            //            {
            //                if (mParts.Name == "Date")
            //                {
            //                    date = mParts.Value;
            //                }
            //                else if (mParts.Name == "From")
            //                {
            //                    from = mParts.Value;
            //                }
            //                else if (mParts.Name == "Subject")
            //                {
            //                    subject = mParts.Value;
            //                }
            //                if (date != "" && from != "")
            //                {
            //                    foreach (MessagePart p in emailInfoResponse.Payload.Parts)
            //                    {
            //                        if (p.MimeType == "text/html")
            //                        {
            //                            byte[] data = FromBase64ForUrlString(p.Body.Data);
            //                            string decodedString = Encoding.UTF8.GetString(data);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //Console.ReadLine();
            #endregion

            //====================================================================================
            //[3] GET LABELS OF EMAIL
            SendHTMLmessage(service);
            Console.ReadLine();

        }

        static byte[] FromBase64ForUrlString(string base64ForUrlInput)
        {
            int padChars = (base64ForUrlInput.Length % 4) == 0 ? 0 : (4 - (base64ForUrlInput.Length % 4));
            StringBuilder result = new StringBuilder(base64ForUrlInput, base64ForUrlInput.Length + padChars);
            result.Append(String.Empty.PadRight(padChars, '='));
            result.Replace('-', '+');
            result.Replace('_', '/');
            return Convert.FromBase64String(result.ToString());
        }

        static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }

        static void SendHTMLmessage(GmailService service)
        {
            //var fromAddress = new MailAddress("thinhgooapi@gmail.com", "From Name");
            ////var toAddress = new MailAddress("thinhifis@gmail.com", "To Name");
            //var toAddress = new MailAddress("thinhgooapi@gmail.com", "To Name");
            //const string fromPassword = "thinhtu710";


            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("thinhgooapi@gmail.com");
            mail.To.Add("thinhgooapi@gmail.com");
            mail.Subject = "Test Mail - 1";
            mail.Body = "mail with attachment";

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment("C:\\Users\\ADMIN\\Pictures\\1.png");
            mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("thinhgooapi@gmail.com", "thinhtu710");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

            //////const string subject = "test";
            //////const string body = "Hey now!!";
            //////var smtp = new SmtpClient
            //////{
            //////    Host = "smtp.gmail.com",
            //////    Port = 587,
            //////    EnableSsl = true,
            //////    DeliveryMethod = SmtpDeliveryMethod.Network,
            //////    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
            //////    Timeout = 20000
            //////};
            //////using (var message = new MailMessage(fromAddress, toAddress)
            //////{
            //////    Subject = subject,
            //////    Body = body
            //////})
            //////{
            //////    smtp.Send(message);
            //////}


            //////const string subject = "Subject";
            //////const string body = "Body";

            //////var smtp = new SmtpClient
            //////{
            //////    Host = "smtp.gmail.com",
            //////    Port = 587,
            //////    EnableSsl = true,
            //////    DeliveryMethod = SmtpDeliveryMethod.Network,
            //////    UseDefaultCredentials = false,
            //////    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            //////};
            //////using (var message = new MailMessage(fromAddress, toAddress)
            //////{
            //////    Subject = subject,
            //////    Body = body
            //////})
            //////{
            //////    smtp.Send(message);
            //////}
        }

        static void SendHTMLmessage_err(GmailService service)
        {
            //Create Message
            MailMessage mail = new MailMessage();
            mail.Subject = "Subject testtttttt!";
            mail.Body = "This is <b><i>body</i></b> of message";
            mail.From = new MailAddress("thinhgooapi@gmail.com");
            mail.IsBodyHtml = true;
            //////string attImg = "C:\\Users\\ADMIN\\Pictures\\1.png";
            //////mail.Attachments.Add(new Attachment(attImg));
            ////////mail.To.Add(new MailAddress("thinhifis@gmail.com"));
            MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);

            Message message = new Message();
            message.Raw = Base64UrlEncode(mimeMessage.ToString());

            ////////Gmail API credentials
            //////UserCredential credential;
            //////using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            //////{
            //////    string credPath = System.Environment.GetFolderPath(
            //////        System.Environment.SpecialFolder.Personal);
            //////    credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart2.json");

            //////    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //////        GoogleClientSecrets.Load(stream).Secrets,
            //////        Scopes,
            //////        "user",
            //////        CancellationToken.None,
            //////        new FileDataStore(credPath, true)).Result;
            //////    Console.WriteLine("Credential file saved to: " + credPath);
            //////}

            //////// Create Gmail API service.
            //////var service = new GmailService(new BaseClientService.Initializer()
            //////{
            //////    HttpClientInitializer = credential,
            //////    ApplicationName = ApplicationName,
            //////});

            //Send Email
            //var result = service.Users.Messages.Send(message, "me/OR UserId/EmailAddress").Execute();
            var result = service.Users.Messages.Send(message, "thinhgooapi@gmail.com").Execute();
        }


        ////public void SendIt()
        ////{
        ////    var msg = new AE.Net.Mail.MailMessage
        ////    {
        ////        Subject = "Your Subject",
        ////        Body = "Hello, World, from Gmail API!",
        ////        From = new MailAddress("[you]@gmail.com")
        ////    };
        ////    msg.To.Add(new MailAddress("yourbuddy@gmail.com"));
        ////    msg.ReplyTo.Add(msg.From); // Bounces without this!!
        ////    var msgStr = new StringWriter();
        ////    msg.Save(msgStr);

        ////    var gmail = new GmailService(MyOwnGoogleOAuthInitializer);
        ////    var result = gmail.Users.Messages.Send(new Message
        ////    {
        ////        Raw = Base64UrlEncode(msgStr.ToString())
        ////    }, "me").Execute();
        ////    Console.WriteLine("Message ID {0} sent.", result.Id);
        ////}

        ////private static string Base64UrlEncode(string input)
        ////{
        ////    var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        ////    // Special "url-safe" base64 encode.
        ////    return Convert.ToBase64String(inputBytes)
        ////      .Replace('+', '-')
        ////      .Replace('/', '_')
        ////      .Replace("=", "");
        ////}




        //////public void SendEmail(MyInternalSystemEmailMessage email)
        //////{
        //////    var mailMessage = new System.Net.Mail.MailMessage();
        //////    mailMessage.From = new System.Net.Mail.MailAddress(email.FromAddress);
        //////    mailMessage.To.Add(email.ToRecipients);
        //////    mailMessage.ReplyToList.Add(email.FromAddress);
        //////    mailMessage.Subject = email.Subject;
        //////    mailMessage.Body = email.Body;
        //////    mailMessage.IsBodyHtml = email.IsHtml;

        //////    foreach (System.Net.Mail.Attachment attachment in email.Attachments)
        //////    {
        //////        mailMessage.Attachments.Add(attachment);
        //////    }

        //////    var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);
        //////    var gmailMessage = new Google.Apis.Gmail.v1.Data.Message
        //////    {
        //////        Raw = Encode(mimeMessage.ToString())
        //////    };

        //////    Google.Apis.Gmail.v1.UsersResource.MessagesResource.SendRequest request = service.Users.Messages.Send(gmailMessage, ServiceEmail);

        //////    request.Execute();
        //////}

        //////public static string Encode(string text)
        //////{
        //////    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);

        //////    return System.Convert.ToBase64String(bytes)
        //////        .Replace('+', '-')
        //////        .Replace('/', '_')
        //////        .Replace("=", "");
        //////}

    }
}