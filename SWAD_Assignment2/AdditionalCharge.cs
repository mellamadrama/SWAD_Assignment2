using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class AdditionalCharge
    {
        public double PenaltyFee { get; set; }
        public double DamageFee { get; set; }
        public double DeliveryFee { get; set; }

        public AdditionalCharge() { }

        public AdditionalCharge(double penaltyFee, double damageFee, double deliveryFee)
        {
            PenaltyFee = penaltyFee;
            DamageFee = damageFee;
            DeliveryFee = deliveryFee;
        }
    }
}
