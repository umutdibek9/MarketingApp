﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class CargoArrivedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
    }
}
