using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Review
    {
        private string rating;
        private DateTime dateCreated;
        private int id;
        private string description;
        private Renter renter;
        private Car car;
        public string Rating
        {
            get { return rating; }
            set { rating = value; }
        }
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public Renter Renter
        {
            get { return renter; }
            set { renter = value; }
        }
        public Car Car
        {
            get { return car; }
            set { car = value; }
        }
        public Review() { }
        public Review(string rating, DateTime dateCreated, int id, string description, Renter renter, Car car)
        {
            this.rating = rating;
            this.dateCreated = dateCreated;
            this.id = id;
            this.description = description;
            this.renter = renter;
            this.car = car;
        }
    }
}
