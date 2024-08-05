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
        public SelfReturn() : base(){ }
        public SelfReturn(DateTime dateTimeReturn, string iCarReturnLocation, AdditionalCharge additionalCharge) : base(additionalCharge)
        {
            this.dateTimeReturn = dateTimeReturn;
            this.iCarReturnLocation = iCarReturnLocation;
        }
    }
}
