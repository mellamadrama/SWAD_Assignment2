using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Pickup : PickUpMethod
    {
        private DateTime dateTimePickup;
        private string pickupLocation;

        public DateTime DateTimePickup
        {
            get { return dateTimePickup; }
            set { dateTimePickup = value; }
        }

        public string PickupLocation
        {
            get { return pickupLocation; }
            set { pickupLocation = value; }
        }

        public Pickup() { }

        public Pickup(DateTime dateTimePickup, string pickupLocation)
        {
            this.dateTimePickup = dateTimePickup;
            this.pickupLocation = pickupLocation;
        }
    }

}
