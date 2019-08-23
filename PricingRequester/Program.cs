using MassTransit;
using MassTransit.Util;
using PricingRequester.Consumers;
using PricingServiceModel.Commands;
using PricingServiceModel.DTOs;
using PricingServiceModel.Events;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;

namespace PricingRequester
{
    class Program
    {
        private static bool _continueRunning = true;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();

            Console.CancelKeyPress += Console_CancelKeyPress;
            var bus = CreateBus();
            Console.WriteLine("Starting Pricing Requester");
            Console.ReadLine();
            while(_continueRunning)
            {
                PricingSpec pricingSpec = new PricingSpec()
                {
                    Symbol = $"Symbol{DateTime.Now.ToString("yyyymmddhhmmss")}",
                    Underlyings = new List<string>() { "AOT" },
                };

                bus.Publish<IPricingRequested>(new { CorrelationId = Guid.NewGuid(), PricingSpec = pricingSpec });
                Console.WriteLine(pricingSpec.ToString());
                Console.ReadLine();
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _continueRunning = false;
        }

        private static IBus CreateBus()
        {
            var rabbitHost = new Uri("rabbitmq://192.168.99.100:5672/saga");
            var user = "guest";
            var password = "guest";
            var inputQueue = "pricing-requester";
            var bus = Bus.Factory.CreateUsingRabbitMq(configurator =>
            {
                var host = configurator.Host(rabbitHost, h =>
                {
                    h.Username(user);
                    h.Password(password);
                });

                configurator.ReceiveEndpoint(host, inputQueue, c =>
                {
                    c.Consumer(() => new PricingProcessedConsumer());
                });
            });

            TaskUtil.Await(() => bus.StartAsync());
            return bus;
        }
    }
}
