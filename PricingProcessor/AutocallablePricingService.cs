using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;
using MassTransit.Util;
using PricingProcessor.Services;
using PricingProcessor.State;
using System;
using System.Collections.Generic;
using System.Text;
using Topshelf;

namespace PricingProcessor
{
    public class AutocallablePricingService : ServiceControl
    {
        private IBusControl _busControl;
        private BusHandle _busHandle;

        public bool Start(HostControl hostControl)
        {
            (this._busControl, this._busHandle) = this.CreateBus();
            return true;
        }

        private (IBusControl, BusHandle) CreateBus()
        {
            var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(this.ConfigureBus);
            var busHandle = TaskUtil.Await(() => bus.StartAsync());
            return (bus, busHandle);
        }

        private void ConfigureBus(IRabbitMqBusFactoryConfigurator factoryConfigurator)
        {
            var rabbitHost = new Uri("rabbitmq://192.168.99.100:5672/saga");
            var inputQueue = "pricing-processor";
            var host = factoryConfigurator.Host(rabbitHost, this.ConfigureCredential);
            factoryConfigurator.ReceiveEndpoint(host, inputQueue, this.ConfigureSagaEndPoint);
        }

        private void ConfigureCredential(IRabbitMqHostConfigurator hostConfigurator)
        {
            var user = "guest";
            var password = "guest";

            hostConfigurator.Username(user);
            hostConfigurator.Password(password);
        }

        private void ConfigureSagaEndPoint(IRabbitMqReceiveEndpointConfigurator endpointConfigurator)
        {
            var stateMachine = new AutocallablePricingStateMachine();
            var repository = new InMemorySagaRepository<AutocallablePricingState>();
            endpointConfigurator.StateMachineSaga(stateMachine, repository);
        }

        public bool Stop(HostControl hostControl)
        {
            this._busHandle?.Stop();
            return true;
        }
    }
}
