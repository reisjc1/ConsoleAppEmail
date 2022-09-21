using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BNP_service
{
    class BNPService
    {

        /// <summary>
        /// Creates an Email with an Excel attached to it.
        /// The BNP data is initialized and processsed inside this method to be injected later inside
        /// a created Excel file in a readable way.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        //public async Task SendEmailayncWithAttachement(EmailMesage message)
        //{
        //    MailMessage mail = new MailMessage();
        //    SmtpClient client = new SmtpClient("owa.konicaminolta.eu", 25);
        //    client.EnableSsl = true;

        //    mail.From = new MailAddress("businessbuilder@konicaminolta.com");
        //    mail.To.Add("joao.silva@konicaminolta.com");
        //    mail.Subject = "Temos email?";
        //    mail.Body = "Aye aye, sir.";
        //    mail.BodyEncoding = System.Text.Encoding.UTF8;
        //    mail.SubjectEncoding = System.Text.Encoding.UTF8;
        //    mail.IsBodyHtml = true;

        //    /*
        //    Attachment data = AcquireDataBNP();
        //    // Add time stamp information for the file.
        //    ContentDisposition disposition = data.ContentDisposition;
        //    disposition.CreationDate = System.IO.File.GetCreationTime(file);
        //    disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
        //    disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
        //    // Add the file attachment to this email message.
        //    mail.Attachments.Add(data);
        //    */



        //    try
        //    {
        //        client.Send(mail);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();

        //    }
        //}



        /// <summary>
        /// Acquires data for usage in BNP's emails.
        /// Every variable used in the Excel file sent is stored in this method.
        /// Each region is defined as a way to identify a concrete purpose for that piece of code.
        /// 
        /// TODO: Implement the code to send emails
        /// 
        /// Financing Type and their codes for quick verification
        /// ID  -   Type            -   code
        /// 1   -   Aquisição       -   0
        /// 2   -   Loc. Financeira -   1
        /// 3   -   Loc. Operacional-   2
        /// 4   -   Loc. Oper. Man. -   3
        /// 6   -   Cessão Créditos -   4
        /// 7   -   Aluguer Direto  -   5
        /// 10  -   Flexpage        -   6
        /// 
        /// </summary>
        /// <returns></returns>
        //public async Task AcquireDataBNP()
        public void AcquireDataBNP()
        {
            #region Variable Declaration
            BNP_Data data = new BNP_Data();

            string KMBS_ID_M;
            string altered_M;
            string gestor_M;
            string sucursal_M;
            double? montante_M;
            string produto_M;
            string subProduto_M;
            double? prazo_M;
            string periodicidade_M;
            string NIF_M;
            string name_M;

            int? financing_code;
            string client_account_number;

            MemoryStream ms;
            #endregion

            using (var db = new BB_DB_DEVEntities())
            {
                #region 1. Database Initialization & Current Date
                List<BB_Proposal_PrazoDiferenciado> bb_p_pd = new List<BB_Proposal_PrazoDiferenciado>();
                BB_Clientes bb_c = new BB_Clientes();
                StringBuilder str_builder = new StringBuilder("");

                int currentDateDay = DateTime.Now.Day;
                int currentDateMonth = DateTime.Now.Month;
                #endregion

                #region 2. Excel file creation and safety checks
                FileInfo file = new FileInfo("C:\\New folder\\TESTE_BNP.xlsx");

                if (file.Exists)
                {
                    file.Delete();
                }
                #endregion

                #region 3. Creating the Excel Package and creating its content
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage(file);

                //Adds a worksheet to the created file
                var worksheet = package.Workbook.Worksheets.Add("BNP Info");

                //Which Excel cells have what data
                worksheet.Cells["A1"].Value = "KMBS ID";
                worksheet.Cells["B1"].Value = "Altera Produto?";
                worksheet.Cells["C1"].Value = "Gestor";
                worksheet.Cells["D1"].Value = "Sucursal";
                worksheet.Cells["E1"].Value = "Montante";
                worksheet.Cells["F1"].Value = "Produto";
                worksheet.Cells["G1"].Value = "Sub Produto";
                worksheet.Cells["H1"].Value = "Prazo";
                worksheet.Cells["I1"].Value = "Periodicidade";
                worksheet.Cells["J1"].Value = "NIF";
                worksheet.Cells["K1"].Value = "Nome da Entidade";
                #endregion

                #region 4. Primary search conditions inside the context & Counter initialization for entry iteration
                //[!] Searches for the current day and the current month and if it is complete
                bb_p_pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.IsComplete == false).Where(x => x.CreatedTime.Value.Day == currentDateDay).Where(x => x.CreatedTime.Value.Month == DateTime.Today.Month).ToList();

                //[!] A "counter" for row progression inside the Excel we're creating
                int i = 1;
                #endregion

                #region 5. Multiple Search of entries within the database context (For Each)
                foreach (var item in bb_p_pd)
                {
                    i++; //The counter increases with each iteration

                    client_account_number = db.BB_Proposal.Where(u => u.ID == item.ProposalID).Select(u => u.ClientAccountNumber).FirstOrDefault();
                    bb_c = db.BB_Clientes.Where(u => u.accountnumber == client_account_number).FirstOrDefault();

                    //Key Variables for Excel Generation
                    #region KMBS_ID
                    string reviewed_proposal_id = item.ProposalID.ToString();
                    int string_length = reviewed_proposal_id.Length;

                    str_builder.Append(reviewed_proposal_id);

                    while (str_builder.Length < 12)
                    {
                        str_builder.Insert(0, "0");
                    }

                    str_builder.Insert(0, "OPP");

                    KMBS_ID_M = str_builder.ToString();
                    #endregion

                    #region Altera produto? ("N" está hardcoded)
                    altered_M = "N";
                    #endregion

                    #region Produto & Sub Produto
                    financing_code = db.BB_FinancingType.Where(f => f.ID == item.FinancingID).Select(f => f.Code).FirstOrDefault();
                    switch (financing_code)
                    {
                        case 0:
                            produto_M = "Locação";
                            subProduto_M = "Locação (S/ Serv)";
                            break;
                        case 1:
                            produto_M = "Aluger Operacional";
                            subProduto_M = "Flexpage (S/ Serv.)";
                            break;
                        case 2:
                            produto_M = "Aluger Operacional";
                            subProduto_M = "Aluguer (S/Serv)";
                            break;
                        case 3:
                            produto_M = "Aluger Operacional";
                            subProduto_M = "Al. Oper Mandatado";
                            break;
                        case 4:
                            produto_M = "Crédito";
                            subProduto_M = "Cessões KM";
                            break;
                        case 6:
                            produto_M = "Aluger Operacional";
                            subProduto_M = "Flexpage (C/ Serv.)";
                            break;
                        default:
                            produto_M = "";
                            subProduto_M = "";
                            break;
                    }
                    #endregion

                    #region Montante, Prazo, Periodicidade, NIF, Nome
                    montante_M = item.ValorFinanciamento;
                    prazo_M = item.PrazoDiferenciado * item.Frequency;
                    periodicidade_M = "M";
                    NIF_M = bb_c.NIF;
                    name_M = bb_c.Name;
                    #endregion

                    #region Gestor & Sucursal
                    var manager_email = db.BB_Proposal.Where(c => c.ID == item.ProposalID).Select(u => u.AccountManager).FirstOrDefault();
                    using (var bb = new masterEntities())
                    {
                        gestor_M = bb.AspNetUsers.Where(x => x.Email == manager_email).Select(x => x.DisplayName).FirstOrDefault();
                        string sucursal = bb.AspNetUsers.Where(x => x.Email == manager_email).Select(x => x.Location).FirstOrDefault();

                        switch (sucursal)
                        {
                            case "Lisboa":
                                sucursal_M = "100861";
                                break;
                            case "Coimbra":
                                sucursal_M = "910861";
                                break;
                            case "Porto":
                                sucursal_M = "900861";
                                break;
                            case "Faro":
                                sucursal_M = "920861";
                                break;
                            default:
                                sucursal_M = "";
                                break;
                        }


                    }
                    #endregion

                    #region Variable Assignement
                    data.KMBS_ID = KMBS_ID_M;
                    data.alteraproduto = altered_M;
                    data.produto = produto_M;
                    data.montante = montante_M;
                    data.prazo = prazo_M;
                    data.periodicidade = periodicidade_M;
                    data.NIF = NIF_M;
                    data.name = name_M;
                    data.gestor = gestor_M;
                    data.sucursal = sucursal_M;
                    data.subproduto = subProduto_M;
                    #endregion

                    #region Progressive row creation (Don't delete the counter)
                    worksheet.Cells["A" + i].Value = data.KMBS_ID;
                    worksheet.Cells["B" + i].Value = data.alteraproduto;
                    worksheet.Cells["C" + i].Value = data.gestor;
                    worksheet.Cells["D" + i].Value = data.sucursal;
                    worksheet.Cells["E" + i].Value = data.montante;
                    worksheet.Cells["F" + i].Value = data.produto;
                    worksheet.Cells["G" + i].Value = data.subproduto;
                    worksheet.Cells["H" + i].Value = data.prazo;
                    worksheet.Cells["I" + i].Value = data.periodicidade;
                    worksheet.Cells["J" + i].Value = data.NIF;
                    worksheet.Cells["K" + i].Value = data.name;

                    #endregion

                    worksheet.Cells.AutoFitColumns();

                    str_builder.Clear();

                }
                #endregion

                #region 6. Package content is saved and further writing is close
                package.Save();
                ms = new MemoryStream(package.GetAsByteArray());
                package.Stream.Close();
                #endregion
            }
            Attachment attachment = new Attachment(ms, "application/vnd.ms-excel");

            //return attachment;
        }


        public class BNPMessage
        {
            public string Destination { get; set; }

            public string Body { get; set; }

            public string Subject { get; set; }

            public string CC { get; set; }
        }

        public class BNP_Data
        {

            public string KMBS_ID { get; set; }

            public string alteraproduto { get; set; }
            public double? montante { get; set; }

            public string produto { get; set; }

            public string subproduto { get; set; }

            public double? prazo { get; set; }

            public string periodicidade { get; set; }

            public string NIF { get; set; }

            public string name { get; set; }

            public string gestor { get; set; }

            public string sucursal { get; set; }
        }
    }

    //public async  TestEmail()
    //{
    //    EmailService emailSend = new EmailService();
    //    EmailMesage message = new EmailMesage();

    //    message.Destination = "joao.silva@konicaminolta.pt";
    //    message.Subject = "Test Table";
    //    StringBuilder strBuilder = new StringBuilder();

    //    #region Table code
    //    string tableStyle = "border: 1px solid #ddd; border-collapse: collapse";
    //    string thStyle = "background-color: #245982;text-align: center;color: white;border: 1px solid #ddd; border-collapse: collapse";

    //    strBuilder.Append("<table style = '" + tableStyle + "'><tr><th style='" + thStyle + "'> Column 1</th><th style='" + thStyle + "'>Column 2</th><th style='" + thStyle + "'>Column 3</th></tr>");
    //    strBuilder.Append("<tr><td style = '" + tableStyle + "'>Value 1</td><td style = '" + tableStyle + "'>Value 2</td><td style = '" + tableStyle + "'>Value 3</td></tr>");
    //    strBuilder.Append("<tr><td style = '" + tableStyle + "'>Value 4</td><td style = '" + tableStyle + "'>Value 5</td><td style = '" + tableStyle + "'>Value 6</td></tr>");
    //    strBuilder.Append("<tr><td style = '" + tableStyle + "'>Value 7</td><td style = '" + tableStyle + "'>Value 8</td><td style = '" + tableStyle + "'>Value 9</td></tr>");
    //    strBuilder.Append("</table>");
    //    message.Body = strBuilder.ToString();
    //    #endregion

    //    await emailSend.SendEmailaync(message);
    //    return View("Index", "Home");
    //}

    //public  void sendEmailWithExcel()
    //{
    //    //EmailService emailSend = new EmailService();
    //    BNPService emailSend = new BNPService();
    //    EmailMesage message = new EmailMesage();

    //    #region Envio do email para o BNP

    //    message.Destination = "joao.silva@konicaminolta.pt";
    //    message.Subject = "Temos email?";

    //    //await emailSend.SendEmailayncWithAttachement(message);
    //    //await emailSend.AcquireDataBNP();

    //    await emailSend.SendEmailayncWithAttachement(message);
    //    #endregion

        
    //}
}

