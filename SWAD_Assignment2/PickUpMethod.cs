using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class PickupMethod
    {
        private Pickup pickup;
        private DeliverCar deliverCar;

        public Pickup Pickup
        {
            get { return pickup; }
            set { pickup = value; }
        }

        public DeliverCar DeliverCar
        {
            get { return deliverCar; }
            set { deliverCar = value; }
        }

        public PickupMethod() { }

        public PickupMethod(Pickup pickup, DeliverCar deliverCar)
        {
            this.pickup = pickup;
            this.deliverCar = deliverCar;
        }
    }

}
