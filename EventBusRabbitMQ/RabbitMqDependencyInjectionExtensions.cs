using AdnTestingService.EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class RabbitMqDependencyInjectionExtensions
{
    // {
    //   "EventBus": {
    //     "SubscriptionClientName": "...",
    //     "RetryCount": 10
    //   }
    // }

    private const string SectionName = "EventBus";

    public static IEventBusBuilder AddRabbitMqEventBus(this IHostApplicationBuilder builder, string connectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton( sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",   
                Port = 5672,              
                UserName = "guest",       
                Password = "guest",      
                AutomaticRecoveryEnabled = true,  // ✅ Enables automatic recovery
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10), // ✅ Retry interval
                DispatchConsumersAsync = true,  // ✅ Async consumer dispatch
                RequestedHeartbeat = TimeSpan.FromSeconds(30) // ✅ Enable heartbeats
            };

            // Create connection asynchronously
            return factory.CreateConnection();
        });

        // RabbitMQ.Client doesn't have built-in support for OpenTelemetry, so we need to add it ourselves
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddSource(RabbitMQTelemetry.ActivitySourceName);
            });

        // Options support
        builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection(SectionName));
        
        // Abstractions on top of the core client API
        builder.Services.AddSingleton<RabbitMQTelemetry>();
        builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
        // Start consuming messages as soon as the application starts
        builder.Services.AddSingleton<IHostedService>(sp => (RabbitMQEventBus)sp.GetRequiredService<IEventBus>());

        return new EventBusBuilder(builder.Services);
    }

    private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }
}