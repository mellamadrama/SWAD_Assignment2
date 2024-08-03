using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Car_Owner : User
    {
        private int license;
        private List<Car> cars;
        public int License
        {
            get { return license; }
            set { license = value; }
        }
        public List<Car> Cars
        {
            get { return cars; }
            set { cars = value; }
        }
        public Car_Owner() { }
        public Car_Owner(int id, string fullName, int contactNum, string email, int password, DateTime dateOfBirth, int license, List<Car> cars)
            : base(id, fullName, contactNum, email, password, dateOfBirth)
        {
            this.license = license;
            this.cars = cars;
        }
        public override string GetRole() => "Car Owner";
    }
}
