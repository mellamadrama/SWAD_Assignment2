using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class CreditCard : PaymentMethod
    {
        public string CardNum { get; set; }
        public string CardName { get; set; }
        public string Bank { get; set; }

        public CreditCard() { }

        public CreditCard(string cardNum, string cardName, string bank)
        {
            CardNum = cardNum;
            CardName = cardName;
            Bank = bank;
        }
    }
}
