using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotineClientEmailSent
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupEmail s = new SetupEmail();
            s.RotineSentEmail();
        }
    }
}
