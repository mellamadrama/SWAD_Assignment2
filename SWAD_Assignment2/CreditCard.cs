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
        public double Balance { get; set; }
        public string Bank { get; set; }

        public CreditCard() { }

        public CreditCard(string cardNum, string cardName, double balance, string bank)
        {
            CardNum = cardNum;
            CardName = cardName;
            Balance = balance;
            Bank = bank;
        }

        public override void DeductBalance(double amount)
        {
            Balance -= amount;
            Console.WriteLine("Payment Successful!");
            Console.WriteLine($"New Account Balance: {(Balance):C}");
        }
    }
}
