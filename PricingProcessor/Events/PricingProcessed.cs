using PricingServiceModel.DTOs;
using PricingServiceModel.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace PricingProcessor.Events
{
    public class PricingProcessed : IPricingProcessed
    {
        public PricingProcessed(Guid correlationId, PricingSpec pricingSpec)
        {
            this.CorrelationId = correlationId;
            this.PricingSpec = pricingSpec;
        }

        public Guid CorrelationId { get; }

        public PricingSpec PricingSpec { get; }
    }
}
