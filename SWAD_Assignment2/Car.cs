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
        private string mileage;
        private string availability;
        private Insurance insurance;
        private List<Booking> bookings;
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
        public string Mileage
        {
            get { return mileage; }
            set { mileage = value; }
        }
        public string Availability
        {
            get { return availability; }
            set { availability = value; }
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
        public Car() { }
        public Car(int carOwnerId, string carPlateNo, string carMake, string model, int year, string mileage, string availability, Insurance insurance, List<Booking> bookings)
        {
            this.carOwnerId = carOwnerId;
            this.carPlateNo = carPlateNo;
            this.carMake = carMake;
            this.model = model;
            this.year = year;
            this.mileage = mileage;
            this.availability = availability;
            this.insurance=insurance;
            this.bookings=bookings;
        }
    }
}
