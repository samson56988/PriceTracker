using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Models
{
    public class ProductionInformation
    {
         public int ProductinformationId { get; set; }

        public int ProductId { get; set; }

        public double value { get; set; }

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

    }
}
