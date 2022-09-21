using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Attachment = System.Net.Mail.Attachment;

namespace ConsoleAppEmail
{
    class EmailService
    {
      
    }



    public class EmailMesage
    {
        public string Destination { get; set; }

        public string Body { get; set; }

        public string Subject { get; set; }

        public string CC { get; set; }
    }
}
