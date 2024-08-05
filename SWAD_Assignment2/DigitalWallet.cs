using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class DigitalWallet : PaymentMethod
    {
        public string Type { get; set; }

        public DigitalWallet() { }

        public DigitalWallet(string type)
        {
            Type = type;
        }
    }
}
