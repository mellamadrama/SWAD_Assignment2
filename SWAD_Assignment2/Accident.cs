using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Accident
    {
        private DateTime dateTimeOccurred;
        private string typeOfAccident;
        private string accidentDescription;

        public DateTime DateTimeOccurred
        {
            get { return dateTimeOccurred; }
            set { dateTimeOccurred = value; }
        }
        public string TypeOfAccident
        {
            get { return typeOfAccident; }
            set { typeOfAccident = value; }
        }
        public string AccidentDescription
        {
            get { return accidentDescription; }
            set { accidentDescription = value; }
        }
        public Accident() { }
        public Accident(DateTime dateTimeOccured, string typeOfAccident, string accidentDescription)
        {
            this.dateTimeOccurred = dateTimeOccured;
            this.typeOfAccident = typeOfAccident;
            this.accidentDescription = accidentDescription;
        }
    }
}
