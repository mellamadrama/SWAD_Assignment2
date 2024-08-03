using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Insurance
    {
        private int carPlateNo;
        private int carOwnerId;
        private DateTime expiryDate;
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
        public Insurance() { }
        public Insurance(int carPlateNo, int carOwnerId, DateTime expiryDate)
        {
            this.carPlateNo=carPlateNo;
            this.carOwnerId=carOwnerId;
            this.expiryDate=expiryDate;
        }
    }
}
