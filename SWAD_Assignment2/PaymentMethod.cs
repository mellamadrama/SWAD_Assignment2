﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    internal abstract class PaymentMethod
    {
        public abstract void DeductBalance(double amount);
    }
}
