using TicketService.API.Shared.Networking;
using TicketService.API.Features.Tickets.SearchTickets;
using System.Reflection;
using TicketService.API.Shared.Behaviors;

namespace TicketService.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITicketSearchApiClient, TicketSearchApiClient>();
        services.AddSingleton<TicketSearch>();
        var CurrentAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(CurrentAssembly).
            RegisterServicesFromAssemblies(CurrentAssembly).AddOpenRequestPreProcessor(typeof(LoggingBehavior<>));
        });
        return services;
    }

    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services)
    {
        return services;
    }
}
