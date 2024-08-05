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
        public double Balance { get; set; }

        public DigitalWallet() { }

        public DigitalWallet(string type, double balance)
        {
            Type = type;
            Balance = balance;
        }
    }
}
