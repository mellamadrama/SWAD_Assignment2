using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    class SelfReturn : ReturnMethod
    {
        private DateTime dateTimeReturn;
        private string iCarReturnLocation;
        private AdditionalCharge additionalCharge;
        public DateTime DateTimeReturn
        {
            get { return dateTimeReturn; }
            set { dateTimeReturn = value; }
        }
        public string ICarReturnLocation
        {
            get { return iCarReturnLocation; }
            set { iCarReturnLocation = value; }
        }
        public AdditionalCharge AdditionalCharge
        {
            get { return additionalCharge; }
            set { additionalCharge = value; }
        }
        public SelfReturn() { }
        public SelfReturn(DateTime dateTimeReturn, string iCarReturnLocation, AdditionalCharge additionalCharge)
        {
            this.dateTimeReturn = dateTimeReturn;
            this.iCarReturnLocation = iCarReturnLocation;
            this.additionalCharge = additionalCharge;
        }
    }
}
