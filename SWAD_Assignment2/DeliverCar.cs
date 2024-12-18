﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal class DeliverCar : PickUpMethod
    {
        private DateTime dateTimeDeliver;
        private string deliveryLocation;
        private double pickupDeliveryFee;

        public DateTime DateTimeDeliver
        {
            get { return dateTimeDeliver; }
            set { dateTimeDeliver = value; }
        }

        public string DeliveryLocation
        {
            get { return deliveryLocation; }
            set { deliveryLocation = value; }
        }

        public double PickupDeliveryFee
        {
            get { return pickupDeliveryFee; }
            set { pickupDeliveryFee = value; }
        }

        public DeliverCar() : base() { }

        public DeliverCar(DateTime dateTimeDeliver, string deliveryLocation, double pickupDeliveryFee, AdditionalCharge additionalCharge) : base(additionalCharge)
        {
            this.dateTimeDeliver = dateTimeDeliver;
            this.deliveryLocation = deliveryLocation;
            this.pickupDeliveryFee = pickupDeliveryFee;
        }
    }
}
