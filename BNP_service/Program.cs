using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNP_service
{
    class Program
    {
        static void Main(string[] args)
        {

            BNPService b = new BNPService();
            b.AcquireDataBNP();
        }
    }
}
