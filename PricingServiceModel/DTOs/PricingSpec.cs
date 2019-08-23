using System;
using System.Collections.Generic;
using System.Text;

namespace PricingServiceModel.DTOs
{
    public class PricingSpec
    {
        public string Symbol { get; set; }
        public IList<string> Underlyings { get; set; }
        public double Premium { get; set; }
    }
}
