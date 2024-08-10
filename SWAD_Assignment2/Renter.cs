using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    class Renter : User
    {
        private int licenseNum;
        private string licenseStatus;
        private int demeritPoints;
        private List<Booking> bookings;

        public int LicenseNum
        {
            get { return licenseNum; }
            set { licenseNum = value; }
        }
        public string LicenseStatus
        {
            get { return licenseStatus; }
            set { licenseStatus = value; }
        }
        public int DemeritPoints
        {
            get { return demeritPoints; }
            set { demeritPoints = value; }
        }
        public List<Booking> Bookings
        {
            get { return bookings; }
            set { bookings = value; }
        }
        public Renter() : base() { }
        public Renter(int id, string fullName, int contactNum, string email, int password, DateTime dateOfBirth,
            int licenseNum, string licenseStatus, int demeritPoints, List<Booking> bookings) : base(id, fullName, contactNum, email, password, dateOfBirth)
        {
            this.licenseNum = licenseNum;
            this.licenseStatus = licenseStatus;
            this.demeritPoints = demeritPoints;
            this.bookings=bookings;
        }
        public override string GetRole() => "Renter";

        //Charlotte Lee S10258027K
        public void AddBooking(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking), "Booking cannot be null");
            }

            bookings.Add(booking);
        }
    }
}