using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Booking
    {
        private string bookingId;
        private DateTime startDate;
        private DateTime endDate;
        private string status;
        private PickUpMethod pickUpMethod;
        private ReturnMethod returnMethod;
        private Payment payment;
        private Car car;
        public string BookingId
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
        public Booking(string bookingId, DateTime startDate, DateTime endDate, string status, PickUpMethod pickUpMethod, ReturnMethod returnMethod, Payment payment, Car car)
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
        public ReturnMethod getReturnMethod()
        {
            return returnMethod;
        }
        public void setDateTimeReturn(DateTime dateTimeReturn)
        {
            SelfReturn selfReturn = (SelfReturn)returnMethod;
            selfReturn.DateTimeReturn = dateTimeReturn;
        }
        public DateTime getEndDate()
        {
            return endDate;
        }
        public void updatePenaltyFee(double penaltyFee)
        {
            payment.AdditionalCharge.PenaltyFee = penaltyFee;
        }

        public void updateTotalFees(double fee)
        {
            payment.TotalFee += fee;
        }
        public string getBookingStatus()
        {
            return status;
        }
        public void updateBookingStatus(string status)
        {
            this.status = status;
            string availability = "Available";
            car.Availability = availability;
        }

        public double getBookingFee()
        {
            return payment.TotalFee;
        }

        public double getDeliveryFee()
        {
            return payment.AdditionalCharge.DeliveryFee;
        }

        public double getPenaltyFee()
        {
            return payment.AdditionalCharge.PenaltyFee;
        }

        public double getDamageFee()
        {
            return payment.AdditionalCharge.DamageFee;
        }


        public static Booking CreateBooking(DateTime startTime, DateTime endTime, string status, PickUpMethod pickUpMethod, ReturnMethod returnMethod, double totalCharge, double totalDeliveryFee, Car selectedCar, Renter renter)
        {
            AdditionalCharge additionalCharge = new AdditionalCharge(0, 0, totalDeliveryFee);
            Booking booking = new Booking
            {
                BookingId = Guid.NewGuid().ToString(),
                StartDate = startTime,
                EndDate = endTime,
                Status = status,
                PickUpMethod = pickUpMethod,
                ReturnMethod = returnMethod,
                Payment = new Payment(DateTime.Now, totalCharge, additionalCharge),
                Car = selectedCar
            };

            renter.AddBooking(booking);

            return booking;
        }
    }
}
