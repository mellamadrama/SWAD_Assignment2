using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Booking
    {
        private int bookingId;
        private DateTime startTimeSlot;
        private DateTime endTimeSlot;
        private string pickupLocation;
        private string returnLocation;
        private bool isDelivery;
        private string status;
        public int BookingId
        {
            get { return bookingId; }
            set { bookingId = value; }
        }
        public DateTime StartTimeSlot
        {
            get { return startTimeSlot; }
            set { startTimeSlot = value; }
        }
        public DateTime EndTimeSlot
        {
            get { return endTimeSlot; }
            set { endTimeSlot = value; }
        }
        public string PickupLocation
        {
            get { return pickupLocation; }
            set { pickupLocation = value; }
        }
        public string ReturnLocation
        {
            get { return returnLocation; }
            set { returnLocation = value; }
        }
        public bool IsDelivery
        {
            get { return isDelivery; }
            set { isDelivery = value; }
        }
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        public Booking() { }
        public Booking(int bookingId, DateTime startTimeSlot, DateTime endTimeSlot, string pickupLocation, string returnLocation, bool isDelivery, string status)
        {
            this.bookingId = bookingId;
            this.startTimeSlot = startTimeSlot;
            this.endTimeSlot = endTimeSlot;
            this.pickupLocation = pickupLocation;
            this.returnLocation = returnLocation;
            this.isDelivery = isDelivery;
            this.status = status;
        }
    }
}
