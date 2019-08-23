using PricingServiceModel.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PricingServiceModel
{
    public interface IMessage
    {
        Guid CorrelationId { get; }

        PricingSpec PricingSpec { get; }
    }
}
