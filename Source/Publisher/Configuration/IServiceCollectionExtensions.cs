using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolaceNEMS.Auth;
using SolaceNEMS.Messaging;
using SolaceNEMS.Messaging.Internal;

namespace SolaceNEMS.Configuration;

public static class IServiceCollectionExtensions
{
    public static void AddNEMS(this IServiceCollection services, IConfigurationSection section)
    {
        services.Configure<NEMSConfig>(section);
        services.AddSingleton<SolaceContextFactory>();
        services.AddSingleton<TokenManager>();

        services.AddSingleton(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<SolaceContextFactory>();
            var context = factory.CreateContext();
            return context;
        });
        
        services.AddSingleton<ISessionManager, SessionManager>();
    }

}