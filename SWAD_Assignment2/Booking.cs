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
        private DateTime startDate;
        private DateTime endDate;
        private string status;
        private PickUpMethod pickUpMethod;
        private ReturnMethod returnMethod;
        private Payment payment;
        public int BookingId
        {
            get { return bookingId; }
            set { bookingId = value; }
        }
        public DateTime StartTimeSlot
        {
            get { return startDate; }
            set { startDate = value; }
        }
        public DateTime EndTimeSlot
        {
            get { return endDate; }
            set { endDate = value; }
        }
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        public PickUpMethod PickUpMethod
        {
            get { return pickUpMethod; }
            set { pickUpMethod = value; }
        }
        public ReturnMethod ReturnMethod
        {
            get { return returnMethod; }
            set { returnMethod = value; }
        }
        public Payment Payment
        {
            get { return payment; }
            set { payment = value; }
        }
        public Booking() { }
        public Booking(int bookingId, DateTime startTimeSlot, DateTime endTimeSlot, string status, PickUpMethod pickUpMethod, ReturnMethod returnMethod, Payment payment)
        {
            this.bookingId = bookingId;
            this.startDate = startTimeSlot;
            this.endDate = endTimeSlot;
            this.status = status;
            this.pickUpMethod = pickUpMethod;
            this.returnMethod=returnMethod;
            this.payment=payment;
        }
    }
}
