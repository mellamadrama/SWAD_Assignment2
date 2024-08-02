using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class iCar_Admin : User
    {
        public iCar_Admin(int id, string fullName, int contactNum, string email, int password, DateTime dateOfBirth)
            : base(id, fullName, contactNum, email, password, dateOfBirth) { }

        public override string GetRole() => "Admin";
    }
}
