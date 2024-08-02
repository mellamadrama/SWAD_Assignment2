using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    class Renter : User
    {
        private int licenseNum;
        private string licenseStatus;
        private int demeritPoints;

        public int LicenseNum
        {
            get { return licenseNum; }
            set { licenseNum = value; }
        }
        public string LicenseStatus
        {
            get { return licenseStatus; }
            set { licenseStatus = value; }
        }
        public int DemeritPoints
        {
            get { return demeritPoints; }
            set { demeritPoints = value; }
        }
        public Renter() : base() { }
        public Renter(int id, string fullName, int contactNum, string email, DateTime dateOfBirth,
            int licenseNum, string licenseStatus, int demeritPoints) : base(id, fullName, contactNum, email, dateOfBirth)
        {
            this.licenseNum = licenseNum;
            this.licenseStatus = licenseStatus;
            this.demeritPoints = demeritPoints;
        }
    }
}