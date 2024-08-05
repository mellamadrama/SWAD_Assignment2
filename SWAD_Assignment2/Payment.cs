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
        public AdditionalCharge AdditionalCharge { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public int TotalFee
        {
            get { return totalFee; }
            set { totalFee = value; }
        }

        public Payment()
        {
            PaymentMethods = new List<PaymentMethod>();
        }

        public Payment(DateTime date, int totalFee, AdditionalCharge additionalCharge, List<PaymentMethod> paymentMethods)
        {
            this.date = date;
            this.totalFee = totalFee;
            AdditionalCharge = additionalCharge;
            PaymentMethods = paymentMethods ?? new List<PaymentMethod>();
        }
    }
}
