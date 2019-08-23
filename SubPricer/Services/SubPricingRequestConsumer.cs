using MassTransit;
using PricingServiceModel.Commands;
using PricingServiceModel.DTOs;
using PricingServiceModel.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubPricer.Services
{
    public class SubPricingRequestConsumer : IConsumer<ISubPricingRequest>
    {
        public async Task Consume(ConsumeContext<ISubPricingRequest> context)
        {
            Console.WriteLine($"Sub Pricing for symbol {context.Message.PricingSpec.Symbol}");
            await Task.Delay(2000);
            this.UpdatePricingSpec(context.Message.PricingSpec);
            await context.Publish<ISubPricingProcessed>(new
            {
                CorrelationId = context.Message.CorrelationId,
                PricingSpec = context.Message.PricingSpec,
            });
        }

        private void UpdatePricingSpec(PricingSpec pricingSpec)
        {
            Random random = new Random();
            double premium = random.Next();
            Console.WriteLine($"Sub Pricing for symbol {pricingSpec.Symbol}, Premium {premium}");
            pricingSpec.Premium = premium;
        }
    }
}
