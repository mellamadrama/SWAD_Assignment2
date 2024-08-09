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
        private float charge;
        private Insurance insurance;
        private List<Booking> bookings;
        private List<string> availableDates;
        private List<string> unavailableDates;
        private List<string> photoList;
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
        public float Charge
        {
            get { return charge; }
            set { charge = value; }
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

        public List<string> PhotoList
        {
            get { return  photoList;}
            set { photoList = value; }
        }
        public Car() { }
        public Car(int carOwnerId, string carPlateNo, string carMake, string model, int year, int mileage, string availability, string insuranceStatus, float charge,Insurance insurance, List<Booking> bookings, List<string> photoList)
        {
            this.carOwnerId = carOwnerId;
            this.carPlateNo = carPlateNo;
            this.carMake = carMake;
            this.model = model;
            this.year = year;
            this.mileage = mileage;
            this.availability = availability;
            this.insuranceStatus = insuranceStatus;
            this.charge = charge;
            this.insurance=insurance;
            this.bookings=bookings;
            this.availableDates = new List<string>();
            this.unavailableDates = new List<string>();
            this.photoList = photoList;
        }

       
    }
}
