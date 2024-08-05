using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Car
    {
        private int carOwnerId;
        private string carPlateNo;
        private string carMake;
        private string model;
        private int year;
        private int mileage;
        private string availability;
        private string insuranceStatus;
        private Insurance insurance;
        private List<Booking> bookings;
        private List<string> availableDates;
        private List<string> unavailableDates;
        public int CarOwnerId
        {
            get { return carOwnerId; }
            set { carOwnerId = value; }
        }
        public string LicensePlate
        {
            get { return carPlateNo; }
            set { carPlateNo = value; }
        }
        public string CarMake
        {
            get { return carMake; } 
            set { carMake = value; }
        }
        public string Model
        {
            get { return model; }
            set { model = value; }
        }
        public int Year
        {
            get { return year; }
            set { year = value; }
        }
        public int Mileage
        {
            get { return mileage; }
            set { mileage = value; }
        }
        public string Availability
        {
            get { return availability; }
            set { availability = value; }
        }
        public string InsuranceStatus
        {
            get { return insuranceStatus; }
            set { insuranceStatus = value; }
        }
        public Insurance Insurance
        {
            get { return insurance; }
            set { insurance = value; }
        }
        public List<Booking> Bookings
        {
            get { return bookings; }
            set { bookings = value; }
        }
        public List<string> AvailableDates
        {
            get { return availableDates; }
            set { availableDates = value; }
        }

        public List<string> UnavailableDates
        {
            get { return unavailableDates; }
            set { unavailableDates = value; }
        }
        public Car() { }
        public Car(int carOwnerId, string carPlateNo, string carMake, string model, int year, int mileage, string availability, string insuranceStatus, Insurance insurance, List<Booking> bookings)
        {
            this.carOwnerId = carOwnerId;
            this.carPlateNo = carPlateNo;
            this.carMake = carMake;
            this.model = model;
            this.year = year;
            this.mileage = mileage;
            this.availability = availability;
            this.insuranceStatus = insuranceStatus;
            this.insurance=insurance;
            this.bookings=bookings;
            this.availableDates = new List<string>();
            this.unavailableDates = new List<string>();
        }
    }
}
