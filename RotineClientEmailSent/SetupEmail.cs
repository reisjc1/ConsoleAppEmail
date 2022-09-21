using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RotineClientEmailSent
{
    class SetupEmail
    {

        public void RotineSentEmail()
        {
            try
            {
                using (var db = new BB_DB_DEVEntities())
                {
                    List<LD_PA5_EmailConfigSent> lstLD_PA5_EmailConfigSent = new List<LD_PA5_EmailConfigSent>();

                    lstLD_PA5_EmailConfigSent = db.LD_PA5_EmailConfigSent.Where(x => x.IsFinish == false).ToList();
                    foreach (var item in lstLD_PA5_EmailConfigSent)
                    {

                        List<LD_PA5_DocumentType> documentType = db.LD_PA5_DocumentType.ToList();
                        List<LD_PA5_DocumentProposal> documentProposal = db.LD_PA5_DocumentProposal.Where(x => x.ContractID == item.ContractID && x.IsToSend == true).ToList();
                        StringBuilder listDoc = new StringBuilder();

                        foreach (var i in documentProposal)
                        {
                            foreach (var f in documentType)
                            {
                                if (f.ID == i.PA5DocumentID)
                                {
                                    listDoc.Append("<b>- " + f.Name + "</b>");
                                    listDoc.Append("</br>");
                                }
                            }
                        }

                        string quote = db.LD_Contrato.Where(x => x.ID == item.ContractID).Select(x => x.QuoteNumber).FirstOrDefault();
                        string nCliente = db.BB_Proposal.Where(x => x.CRM_QUOTE_ID == quote).Select(x => x.ClientAccountNumber).FirstOrDefault();

                        //if(DateTime.Compare(item.NextDateSent.GetValueOrDefault(), DateTime.Now) == 0)
                        if (true)
                        {
                            LD_Email_Log _log = new LD_Email_Log();
                            _log.ContractID = item.ContractID;
                            _log.QuoteNumber = quote;
                            _log.ProcessDate = DateTime.Now;
                            _log.NrClient = nCliente;
                            _log.Status = "Enviado";
                            
                            if (item.Nr_Reminder != 3)
                                item.IsStarted = true;


                            item.Nr_Reminder -= 1;
                            item.NextDateSent = item.NextDateSent.GetValueOrDefault().AddDays(1);

                            EmailMesage email1 = new EmailMesage();
                            email1.Destination = "joao.reis@konicaminolta.pt";
                            email1.Subject = (item.IsStarted.GetValueOrDefault() ? "Lembrete: " : "") + "Solicitação de documentos para seguimento do processo - Quote:" + quote + " nrCliente:" + nCliente;

                            if (item.Mode == 2)
                            {
                                email1.Body = "<b>Caro(a) Cliente,<b>" +
                                                                "</br>" +
                                                                 "</br>" +
                                                                "<b>Bem-vindo à Konica Minolta!</b>" +
                                                                "</br>" +
                                                                "<b>Agradecemos desde já a confiança por ter optado pelos nossos serviços, pelo que tudo faremos para ir ao encontro das suas melhores expectativas!</b>" +
                                                                "</br>" +
                                                                 "</br>" +
                                                                "<b>Para darmos o melhor seguimento, do processo da vossa estimada organização, pedimos o envio da documentação constante da lista abaixo.</b>" +
                                                                 "</br>" +
                                                                  "</br>" +
                                                                 "<b>Lista de documentos a remeter:</b>" +
                                                                  "</br>" +
                                                                  listDoc.ToString() +
                                                                   "</br>" +
                                                                   "<b>Para enviar-nos os documentos, bastará responder a este email com os documentos solicitados em anexo. Pedimos o favor de não alterar o “Assunto/Subject”.</b>" +
                                                                     "</br>" +
                                                                      "</br>" +
                                                                      "<b>Ao seu dispor," +
                                                                        "</br>" +
                                                                         "<b>Konica Minolta";
                            }

                            if (item.Mode == 3)
                            {
                            }

                            SendEmailaync(email1);

                            _log.body = email1.Body;
                            _log.Subject = email1.Subject;
                            _log.EmailSent = email1.Destination;
                            //TODO: Gravar para historico o envio do email.
                            

                            if (item.Nr_Reminder == 0)
                            {
                                //item.IsFinish = true;
                            }

                            item.NextDateSent = item.NextDateSent.GetValueOrDefault().AddDays(3);

                            db.Entry(item);
                            db.LD_Email_Log.Add(_log);
                            db.SaveChanges();
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }

        public async Task SendEmailaync(EmailMesage message)
        {

            SmtpClient client = new SmtpClient("owa.konicaminolta.eu", 25);
            client.EnableSsl = true;


            MailMessage message1 = new MailMessage();
            message1.From = new MailAddress("documentos@konicaminolta.pt", String.Empty, System.Text.Encoding.UTF8);
            List<string> destinationArray = message.Destination.Split(';').ToList();
            foreach (string s in destinationArray)
            {
                message1.To.Add(new MailAddress(s, String.Empty, System.Text.Encoding.UTF8));
            }

            //message1.Bcc.Add("bb@konicaminolta.pt");
            message1.Body = message.Body;

            if (message.CC != null && message.CC.Length > 0)
                message1.CC.Add(message.CC);

            message1.BodyEncoding = System.Text.Encoding.UTF8;
            message1.Subject = message.Subject;
            message1.SubjectEncoding = System.Text.Encoding.UTF8;
            message1.IsBodyHtml = true;

            try
            {
                client.Send(message1);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }


        }

        public class EmailMesage
        {
            public string Destination { get; set; }

            public string Body { get; set; }

            public string Subject { get; set; }

            public string CC { get; set; }
        }
    }
}
