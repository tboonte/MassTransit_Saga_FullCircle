using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Util;
using SubPricer.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Topshelf;

namespace SubPricer
{
    public class SubPricingRequestService : ServiceControl
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
            var inputQueue = "sub-pricer";
            var host = factoryConfigurator.Host(rabbitHost, this.ConfigureCredential);
            factoryConfigurator.ReceiveEndpoint(host, inputQueue, this.ConfigureReceiveEndPoint);
        }

        private void ConfigureCredential(IRabbitMqHostConfigurator hostConfigurator)
        {
            var user = "guest";
            var password = "guest";

            hostConfigurator.Username(user);
            hostConfigurator.Password(password);
        }

        private void ConfigureReceiveEndPoint(IRabbitMqReceiveEndpointConfigurator endpointConfigurator)
        {
            endpointConfigurator.Consumer(() => new SubPricingRequestConsumer());
        }

        public bool Stop(HostControl hostControl)
        {
            this._busHandle?.Stop();
            return true;
        }
    }
}
