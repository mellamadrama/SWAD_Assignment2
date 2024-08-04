using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class ReturnMethod
    {
        private SelfReturn selfReturn;
        private DeliveryReturn deliveryReturn;

        public SelfReturn SelfReturn
        {
            get { return selfReturn; }
            set { selfReturn = value; }
        }

        public DeliveryReturn DeliveryReturn
        {
            get { return deliveryReturn; }
            set { deliveryReturn = value; }
        }

        public ReturnMethod() { }

        public ReturnMethod(SelfReturn selfReturn, DeliveryReturn deliveryReturn)
        {
            this.selfReturn = selfReturn;
            this.deliveryReturn = deliveryReturn;
        }
    }

}
