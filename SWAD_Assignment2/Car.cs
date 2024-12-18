﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Car
    {
        private int carOwnerId;
        private string licencePlate;
        private string carMake;
        private string model;
        private int year;
        private int mileage;
        private string availability;
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
            get { return licencePlate; }
            set { licencePlate = value; }
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
        public Car(int carOwnerId, string licencePlate, string carMake, string model, int year, int mileage, string availability, float charge,Insurance insurance, List<Booking> bookings, List<string> photoList)
        {
            this.carOwnerId = carOwnerId;
            this.licencePlate = licencePlate;
            this.carMake = carMake;
            this.model = model;
            this.year = year;
            this.mileage = mileage;
            this.availability = availability;
            this.charge = charge;
            this.insurance=insurance;
            this.bookings=bookings;
            this.availableDates = new List<string>();
            this.unavailableDates = new List<string>();
            this.photoList = photoList;
        }
        // Isabelle Tan S10257093F
        public void setAvailability(string avail)
        {
            availability = avail;
        }
        public List<string> getAvailableDates()
        {
            return availableDates.Except(unavailableDates).ToList();
        }
        public List<string> getUnavailableDates()
        {
            return unavailableDates;
        }
        public void updateCarAvailability(string startDateTime, string endDateTime, List<string> availableDates)
        {
            int startIndex = availableDates.FindIndex(date => date == startDateTime);
            int endIndex = availableDates.FindIndex(date => date == endDateTime);

            if (startIndex != -1 && endIndex != -1 && startIndex <= endIndex)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    unavailableDates.Add(availableDates[i]);
                }

                availableDates.RemoveRange(startIndex, endIndex - startIndex + 1);
            }
        }
        public void resetCarAvailability(List<string> originalAvailableDates, List<string> originalUnavailableDates)
        {
            AvailableDates = new List<string>(originalAvailableDates);
            UnavailableDates = new List<string>(originalUnavailableDates);
        }
    }
}
