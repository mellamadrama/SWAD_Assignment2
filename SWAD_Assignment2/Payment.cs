using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Payment
    {
        private DateTime date;
        private int totalFee;
        public DateTime Date 
        { 
            get { return date; } 
            set {  date = value; } 
        }
        public int TotalFee
        {
            get { return totalFee; }
            set { totalFee = value; }
        }
        public Payment() { }
        public Payment(DateTime date, int totalFee)
        {
            this.date = date;
            this.totalFee = totalFee;
        }
    }
}
