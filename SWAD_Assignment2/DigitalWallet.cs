using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class DigitalWallet : PaymentMethod
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double Balance { get; set; }

        public DigitalWallet() { }

        public DigitalWallet(string name, string type, double balance)
        {
            Name = name;
            Type = type;
            Balance = balance;
        }
        public override void DeductBalance(double amount)
        {
            Balance -= amount;
            Console.WriteLine("Payment Successful!");
            Console.WriteLine($"New Account Balance: {(Balance):C}");
        }
    }
}
