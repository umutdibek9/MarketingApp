﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class CargoFailedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public string Message { get; set; }
        public decimal Price { get; set; }
        public List<OrderItemMessage> orderItems { get; set; }
    }
}
