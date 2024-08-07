using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Insurance
    {
        private DateTime expiryDate;
        private Car car;
        private Insurance_Company company;

        public DateTime ExpiryDate
        {
            get { return expiryDate; }
            set { expiryDate = value; }
        }
        public Car Car
        {
            get { return car; }
            set { car = value; }
        }
        public Insurance_Company Company
        {
            get { return company; }
            set { company = value; }
        }
        public Insurance() { }
        public Insurance(DateTime expiryDate, Car car, Insurance_Company company)
        {
            this.expiryDate = expiryDate; 
            this.car = car;
            this.company = company;
        }
    }
}
