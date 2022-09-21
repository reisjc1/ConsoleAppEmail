using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppEmail
{
    class RoutineEmail
    {
        public async void InitSearchEmailAsync()
        {
            try
            {

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("RoutineEmail11111" + "-Start", EventLogEntryType.Information, 101, 1);
                }
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
                // Set specific credentials.
                //service.UseDefaultCredentials = false;
                service.Credentials = new WebCredentials("joao.reis@konicaminolta.pt", "konotirbca012!");
                //service.Credentials = new WebCredentials("joao.reis@konicaminolta.pt", "konotirfffbca03!","konicaminoltabpt.onmicrosoft.com");


                //// Look up the user's EWS endpoint by using Autodiscover.
                //service.TraceEnabled = true;

                service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
                //service.AutodiscoverUrl("joao.reis@konicaminolta.pt", RedirectionUrlValidationCallback);
                //service.AutodiscoverUrl("joao.reis@konicaminolta.pt", RedirectionCallback);



                FolderId FolderToAccess = new FolderId(WellKnownFolderName.Inbox, "bb@konicaminolta.pt");

                //FindFoldersResults folderSearchResults = service.FindFolders(WellKnownFolderName.Inbox, );
                SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));

                FindItemsResults<Item> findResults = service.FindItems(
                   FolderToAccess, sf,
                   new ItemView(10));

                foreach (Item item in findResults.Items)
                {

                    string quoteNumber = ExtractQuote(item.Subject);
                    //bool IsApproved = ExtractIsApproved(item.Subject);

                    bool isSaveStatus = false;
                    //await EnviarEmailSucessoAsync(((Microsoft.Exchange.WebServices.Data.EmailMessage)item).From.Address, item.InReplyTo);


                    using (var db1 = new BB_DB_DEVEntities())
                    {
                        if (quoteNumber != null)
                        {
                            List<BB_Proposal> list1 = db1.BB_Proposal.Where(x => x.CRM_QUOTE_ID == quoteNumber).ToList();


                            foreach (var b in list1)
                            {
                                b.StatusID = 5;
                                b.StatusCRM1 = "Active-Approved";
                                db1.Entry(b).State = EntityState.Modified;
                                db1.SaveChanges();
                                isSaveStatus = true;
                            }
                        }

                    }


                    if (isSaveStatus)
                    {
                        var emailProps = new PropertySet(ItemSchema.MimeContent, ItemSchema.Body,
                                ItemSchema.InternetMessageHeaders);
                        var email = EmailMessage.Bind(service, item.Id, emailProps);

                        //await EnviarEmailSucesso1(email);


                        MessageBody mBody = new MessageBody();
                        StringBuilder strBuilder = new StringBuilder();
                        strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                        strBuilder.Append("<br/>");
                        //strBuilder.Append("O seu pedido de " + tipodeaprovacao + "para " + prazoDif.PrazoDiferenciado + " mes(es), para seguinte o seguinte " + nameCliente + " foi " + fraseAprovado + ".");
                        strBuilder.Append("Business Builder - Por favor, não responder a este email.");

                        mBody.Text = strBuilder.ToString();
                        mBody.BodyType = BodyType.HTML;

                        //email.Service.
                        //email.From = "jorge.colaco@konicaminolta.pt";
                        email.Reply(mBody, false);

                        var emailMatching = email;
                        try
                        {
                            email.IsRead = true;
                            email.Update(ConflictResolutionMode.AutoResolve);
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("RoutineEmail11111" + ex.Message.ToString(), EventLogEntryType.Information, 101, 1);
                }
            }
        }

        private async System.Threading.Tasks.Task EnviarEmailSucesso1(EmailMessage message)
        {
            try
            {
                SmtpClient client = new SmtpClient("owa.konicaminolta.eu", 25);
                client.EnableSsl = true;
                //client.Credentials = new System.Net.NetworkCredential("joao.reis@konicaminolta.pt", "Password123");
                MailAddress from = new MailAddress("bb@konicaminolta.pt", String.Empty, System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress("joao.reis@konicaminolta.pt");

                MailMessage message1 = new MailMessage(from, to);
                //string id = source.Headers["Message-ID"];
                message1.Headers.Add("In-Reply-To", message.ConversationId);

                //    //Try to get 'References' header from the source and add it to the reply
                //string references = message.Headers["References"];
                //    if (!string.IsNullOrEmpty(references))
                //        references += ' ';





                message1.Subject = "";

                //message1.Bcc.Add("bb@konicaminolta.pt");
                message1.Body = "";

                //if (message.CC != null && message.CC.Length > 0)
                //    message1.CC.Add(message.CC);


                //message1.Headers.Add("In-Reply-To", "<Message-ID Value>");

                message1.BodyEncoding = System.Text.Encoding.UTF8;
                //message1.Subject = message.Subject;
                message1.SubjectEncoding = System.Text.Encoding.UTF8;
                message1.IsBodyHtml = true;


                client.Send(message1);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }

        public void EnviarEmailSucesso(EmailMessage e)
        {

            //EMAIL SEND
            EmailService emailSend = new EmailService();
            //EmailMesage message = new EmailMesage();

            //message.Destination = dest;
            //message.Subject = subj;
            ////message.Subject = "Business Team - ";
            //StringBuilder strBuilder = new StringBuilder();
            //strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
            //strBuilder.Append("<br/>");
            ////strBuilder.Append("O seu pedido de " + tipodeaprovacao + "para " + prazoDif.PrazoDiferenciado + " mes(es), para seguinte o seguinte " + nameCliente + " foi " + fraseAprovado + ".");
            //strBuilder.Append("Business Builder - Por favor, não responder a este email.");
            //message.Body = strBuilder.ToString();

            //emailSend.SendEmailaync(e);
        }



        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;
            Uri redirectionUri = new Uri(redirectionUrl);
            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }

        private bool ExtractIsApproved(string subject)
        {
            bool isApproved = subject.Contains("foi aprovada");

            return isApproved;

        }

        private string ExtractQuote(string subject)
        {
            string quoteNumber = null;
            if (subject.Contains("QU2-"))
            {
                 quoteNumber = subject.Substring(subject.IndexOf("QU2"), 17);
            }
            return quoteNumber;
        }


        public void teste1111()
        {
            ExchangeService _service;

            try
            {
                Console.WriteLine("Registering Exchange connection");

                _service = new ExchangeService
                {
                    Credentials = new WebCredentials("joao.reis@konicaminolta.pt", "konotirbca05!")
                };
            }
            catch
            {
                Console.WriteLine("new ExchangeService failed. Press enter to exit:");
                return;
            }

            // This is the office365 webservice URL
            _service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

            // Prepare seperate class for writing email to the database
            try
            {


                Console.WriteLine("Reading mail");

                // Read 100 mails
                foreach (EmailMessage email in _service.FindItems(WellKnownFolderName.Inbox, new ItemView(100)))
                {


                }

                Console.WriteLine("Exiting");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error has occured. \n:" + e.Message);
            }
        }
    }
}
