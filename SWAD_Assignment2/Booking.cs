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
        private Car car;
        public int BookingId
        {
            get { return bookingId; }
            set { bookingId = value; }
        }
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        public DateTime EndDate
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
        public Car Car
        {
            get { return car; }
            set { car = value; }
        }
        public Booking() { }
        public Booking(int bookingId, DateTime startDate, DateTime endDate, string status, PickUpMethod pickUpMethod, ReturnMethod returnMethod, Payment payment, Car car)
        {
            this.bookingId = bookingId;
            this.startDate = startDate;
            this.endDate = endDate;
            this.status = status;
            this.pickUpMethod = pickUpMethod;
            this.returnMethod = returnMethod;
            this.payment = payment;
            this.car = car;
        }

        public void updatePenaltyFee(double penaltyFee)
        {
            payment.AdditionalCharge.PenaltyFee = penaltyFee;
        }

        public void updateTotalFees(double fee)
        {
            payment.TotalFee += fee;
        }

        public void updateBookingStatus(string status)
        {
            this.status = status;
            string availability = "Available";
            car.Availability = availability;
        }


    }
}
