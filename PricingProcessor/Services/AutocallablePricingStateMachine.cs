using Automatonymous;
using Automatonymous.Binders;
using MassTransit.Saga;
using PricingProcessor;
using PricingProcessor.Events;
using PricingProcessor.State;
using PricingServiceModel;
using PricingServiceModel.Commands;
using PricingServiceModel.DTOs;
using PricingServiceModel.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SagaState = Automatonymous.State;

namespace PricingProcessor.Services
{
    public class AutocallablePricingStateMachine : MassTransitStateMachine<AutocallablePricingState>
    {
        public AutocallablePricingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            this.Event(() => this.PricingRequested, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            this.Event(() => this.SubPricingProcessed, x => x.CorrelateById(c => c.Message.CorrelationId));

            Initially(
                When(PricingRequested)
                .Then(context =>
                {
                    this.UpdateSagaState(context.Instance, context.Data.PricingSpec);
                })
                .Then(InterceptPricingRequested)
                .ThenAsync(context => this.SendCommand<ISubPricingRequest>("sub-pricer", context))
                .TransitionTo(Processing));

            During(Processing,
                When(SubPricingProcessed)
                .Then(context =>
                {
                    InterceptSubPricingProcessed(context);
                })
                .Publish(context => new PricingProcessed(context.Data.CorrelationId, context.Data.PricingSpec))
                .Finalize());

            SetCompletedWhenFinalized();
        }

        private void InterceptPricingRequested(BehaviorContext<AutocallablePricingState, IPricingRequested> obj)
        {
            Console.WriteLine($"Sending ISubPricingRequest Command Correlation {obj.Data.CorrelationId}");
        }

        private void InterceptSubPricingProcessed(BehaviorContext<AutocallablePricingState, ISubPricingProcessed> obj)
        {
            Console.WriteLine($"Receiving ISubPricingProcessed Event Correlation {obj.Data.CorrelationId}");
        }

        private void UpdateSagaState(AutocallablePricingState state, PricingSpec pricingSpec)
        {
            var currentDate = DateTime.Now;
            state.PricingSpec = pricingSpec;
        }

        private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<AutocallablePricingState, IMessage> context)
            where TCommand : class, IMessage
        {
            var sendEndPoint = await context.GetSendEndpoint(new Uri($"rabbitmq://192.168.99.100:5672/saga/{endpointKey}"));
            await sendEndPoint.Send<TCommand>(new
            {
                CorrelationId = context.Data.CorrelationId,
                PricingSpec = context.Data.PricingSpec,
            });
        }

        public SagaState Processing { get; private set; }
        public Event<IPricingRequested> PricingRequested { get; private set; }
        public Event<ISubPricingProcessed> SubPricingProcessed { get; private set; }
    }
}
