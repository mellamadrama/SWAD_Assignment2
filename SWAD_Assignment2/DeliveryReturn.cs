using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class DeliveryReturn : ReturnMethod
    {
        private DateTime dateTimeReturnDelivery;
        private string returnLocation;
        private double returnDeliveryFee;

        public DateTime DateTimeReturnDelivery
        {
            get { return dateTimeReturnDelivery; }
            set { dateTimeReturnDelivery = value; }
        }

        public string ReturnLocation
        {
            get { return returnLocation; }
            set { returnLocation = value; }
        }

        public double ReturnDeliveryFee
        {
            get { return returnDeliveryFee; }
            set { returnDeliveryFee = value; }
        }

        public DeliveryReturn() { }

        public DeliveryReturn(DateTime dateTimeReturnDelivery, string returnLocation, double returnDeliveryFee)
        {
            this.dateTimeReturnDelivery = dateTimeReturnDelivery;
            this.returnLocation = returnLocation;
            this.returnDeliveryFee = returnDeliveryFee;
        }
    }

}
