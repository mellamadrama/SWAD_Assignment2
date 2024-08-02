using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class Car_Owner
    {
        private int licence;
        public int Liscence
        {
            get { return licence; }
            set { licence = value; }
        }
        public Car_Owner() { }
        public Car_Owner(int license)
        {
           this.licence = licence;
        }
    }
}
