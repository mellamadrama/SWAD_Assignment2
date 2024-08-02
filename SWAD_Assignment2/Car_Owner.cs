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
        private int licence;
        public int Liscence
        {
            get { return licence; }
            set { licence = value; }
        }
        public Car_Owner() { }
        public Car_Owner(int id, string fullName, int contactNum, string email, int password, DateTime dateOfBirth, int license)
            : base(id, fullName, contactNum, email, password, dateOfBirth)
        {
            this.licence = license;
        }
        public override string GetRole() => "Car Owner";
    }
}
