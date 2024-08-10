using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class InsuranceCompany
    {
        private int branchNo;   
        private string companyName;
        private string telephone;
        private string address;
        private string emailAddress;

        public int BranchNo 
        { 
            get { return branchNo; } 
            set { branchNo = value; } 
        }
        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }
        public InsuranceCompany() { }
        public InsuranceCompany(int branchNo, string companyName ,string telephone, string address, string emailAddress)
        {
            this.branchNo = branchNo;
            this.companyName = companyName;
            this.telephone = telephone;
            this.address = address;
            this.emailAddress = emailAddress;
        }
    }
}
