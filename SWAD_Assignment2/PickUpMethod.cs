using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    abstract class PickUpMethod
    {
        private AdditionalCharge additionalCharge;
        public AdditionalCharge AdditionalCharge
        {
            get { return additionalCharge; }
            set { additionalCharge = value; }
        }
        public PickUpMethod() { }
        public PickUpMethod(AdditionalCharge additionalCharge)
        {
            this.additionalCharge = additionalCharge;
        }
    }

}
