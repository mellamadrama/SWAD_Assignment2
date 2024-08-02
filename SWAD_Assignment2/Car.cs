using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Car
    {
        private string carMake;
        private string model;
        private int year;
        private int mileage;
        //private .. photo;
        private string availability;

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
        public Car() { }
        public Car(string carMake, string model, int year, int mileage, string availability)
        {
            this.carMake = carMake;
            this.model = model;
            this.year = year;
            this.mileage = mileage;
            this.availability = availability;
        }
    }
}
