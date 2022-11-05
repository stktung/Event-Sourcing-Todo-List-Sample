using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SleekFlow.Todo.Infrastructure.EventSubscription
{
    public static class EventSubscriptionServiceCollectionExtensions
    {
        public static IServiceCollection AddEventSubscription<TBackgroundService>(
            this IServiceCollection services,
            Action<EventSubscriptionOptions> configure) where TBackgroundService : BackgroundService
        {
            if (configure is null)
                throw new ArgumentNullException(nameof(configure));

            var options = new EventSubscriptionOptions();
            configure(options);
            
            return services
                .Configure(configure)
                .Configure<EventSubscriptionHandlerOptions>(opts =>
                {
                    var operationOptions = EventStoreClientOperationOptions.Default;

                    opts.Client = new EventStorePersistentSubscriptionsClient(new EventStoreClientSettings
                    {
                        ConnectionName = options.ApplicationName,
                        DefaultCredentials = new UserCredentials(options.Username, options.Password),
                        OperationOptions = operationOptions
                    });

                    opts.Settings = EventStore.ClientAPI.PersistentSubscriptionSettings.Create().ResolveLinkTos().Build();
                    
                    opts.StreamName = options.SubscribeToStream;
                    opts.GroupName = "Group1";
                    opts.RetryDelay = TimeSpan.FromSeconds(1);
                    opts.RetryReconnectDelay = TimeSpan.FromSeconds(1);

                    opts.UserName = options.Username;
                    opts.Password = options.Password;
                })
                .AddHostedService<TBackgroundService>();
        }
    }
}