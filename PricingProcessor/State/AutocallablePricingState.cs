using Automatonymous;
using PricingServiceModel.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PricingProcessor.State
{
    public class AutocallablePricingState : SagaStateMachineInstance
    {
        public AutocallablePricingState(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }

        public string CurrentState { get; set; }
        public PricingSpec PricingSpec { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
