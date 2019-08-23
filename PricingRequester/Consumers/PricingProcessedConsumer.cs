using MassTransit;
using PricingServiceModel.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PricingRequester.Consumers
{
    public class PricingProcessedConsumer : IConsumer<IPricingProcessed>
    {
        public async Task Consume(ConsumeContext<IPricingProcessed> context)
        {
            Console.WriteLine($"Pricing processed Symbol = {context.Message.PricingSpec.Symbol}, Premium {context.Message.PricingSpec.Premium}");
        }
    }
}
