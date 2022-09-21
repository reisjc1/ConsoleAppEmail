using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotineClientEmailReceive
{
    class ReceiveEmail
    {
        public void RotineReceiveEmail()
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {


                    //string subject = "sfidshfiu fuhdsufids Quote:Hello-worldjjd.aaaasd ncliente: 4234234";

                    //int space1 = subject.IndexOf("Quote:")+6;
                    //int space2 = subject.IndexOf("ncliente")-1;
                    //string firstPart = subject.Substring(space1, space2-space1);
                    //string firstPart = subject.Substring(0, subject.IndexOf(' ', subject.IndexOf(' ') + 1));


                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Rotina Leasedesk " + "- Start", EventLogEntryType.Information, 101, 1);

                    ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
                    // Set specific credentials.
                    //service.UseDefaultCredentials = false;
                    service.Credentials = new WebCredentials("joao.reis@konicaminolta.pt", "konotirbca012!");

                    service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

                    FolderId FolderToAccess = new FolderId(WellKnownFolderName.Inbox, "joao.reis@konicaminolta.pt");

                    //FindFoldersResults folderSearchResults = service.FindFolders(WellKnownFolderName.Inbox, );
                    SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));

                    FindItemsResults<Item> findResults = service.FindItems(
                       FolderToAccess, sf,
                       new ItemView(30));

                    foreach (Item item in findResults.Items)
                    {
                        if (item.Subject.Contains("Quote"))
                        {
                            //grava aqui o nt attachments item.HasAttachments || item.Attachments
                            string subject = item.Subject;
                            int space1 = subject.IndexOf("Quote:") + 6;
                            int space2 = subject.IndexOf("ncliente") - 1;
                            string firstPart = subject.Substring(space1, space2 - space1);


                            item.Load();
                        }
                       

                        try
                        {
                            var emailProps = new PropertySet(ItemSchema.MimeContent, ItemSchema.Body,
                            ItemSchema.InternetMessageHeaders);
                            var email = EmailMessage.Bind(service, item.Id, emailProps);
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
                ex.Message.ToString();
            }
        }
    }
}
