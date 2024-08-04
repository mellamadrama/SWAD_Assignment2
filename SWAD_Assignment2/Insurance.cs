using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Insurance : Insurance_Company
    {
        private int carPlateNo;
        private int carOwnerId;
        private DateTime expiryDate;
        private List<Insurance> insurance;
        public int CarPlateNo
        {
            get { return carPlateNo; }
            set { carPlateNo = value; }
        }
        public int CarOwnerId
        {
            get { return carOwnerId; }
            set { carOwnerId = value; }
        }
        public DateTime ExpiryDate
        {
            get { return expiryDate; }
            set { expiryDate = value; }
        }
        public List<Insurance> Insurances
        {
            get { return insurance; }
            set { insurance = value; }
        }
        public Insurance() { }
        public Insurance(int branchNo, string companyName, string telephone, string address, string emailAddress, int carPlateNo, int carOwnerId, DateTime expiryDate, List<Insurance> insurance)
            :base(branchNo, companyName, telephone, address, emailAddress)
        {
            this.carPlateNo = carPlateNo;
            this.carOwnerId = carOwnerId;
            this.expiryDate = expiryDate;
        }
    }
}
