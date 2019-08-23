using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SubPricer.Services;
using System;
using Topshelf;

namespace SubPricer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();

            HostFactory.Run(x => x.Service<SubPricingRequestService>());
        }
    }
}
