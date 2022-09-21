using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotineClientEmailReceive
{
    class Program
    {
        static void Main(string[] args)
        {
            ReceiveEmail e = new ReceiveEmail();
            e.RotineReceiveEmail();
        }
    }
}
