using TicketService.API.Features.Tickets.SearchTickets;
using System.Reflection;
using TicketService.API.Shared.Behaviors;
using System.Text;

namespace TicketService.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<TicketSearch>();
        var CurrentAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(CurrentAssembly)
              .AddOpenRequestPreProcessor(typeof(LoggingBehavior<>));
        });
        return services;
    }

    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services)
    {
        return services;
    }

    public static void LogAssemblyInfoToFile()
    {
        try
        {
            var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }

            var logFilePath = Path.Combine(logsDirectory, "assembly-info.txt");
            var assemblyInfo = new StringBuilder();

            assemblyInfo.AppendLine("Ticket Service Minimal API Assembly Information");
            assemblyInfo.AppendLine($"Generated at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            assemblyInfo.AppendLine();

            var currentAssembly = Assembly.GetExecutingAssembly();
            LogAssemblyDetails(assemblyInfo, currentAssembly, "Current Assembly (TicketService.API)");

            LogMediatRHandlers(assemblyInfo, currentAssembly);

            File.WriteAllText(logFilePath, assemblyInfo.ToString());
            Console.WriteLine($"Assembly information logged to: {logFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log assembly information: {ex.Message}");
        }
    }

    private static void LogAssemblyDetails(StringBuilder sb, Assembly assembly, string title)
    {
        sb.AppendLine($"{title}");
        sb.AppendLine($"Full Name: {assembly.FullName}");
        sb.AppendLine($"Location: {assembly.Location}");
        sb.AppendLine();
    }

    private static void LogMediatRHandlers(StringBuilder sb, Assembly assembly)
    {
        sb.AppendLine("API Components Found");
        
        try
        {
            var types = assembly.GetTypes();
            
            // Find all public classes (your API components)
            var publicClasses = types.Where(t => t.IsClass && t.IsPublic && !t.IsAbstract).ToList();
            sb.AppendLine($"Public classes: {publicClasses.Count}");
            
            // Find endpoints
            var endpoints = publicClasses.Where(t => t.Name.Contains("Endpoint") || t.Name.Contains("Search")).ToList();
            sb.AppendLine($"Endpoints: {endpoints.Count}");
            foreach (var endpoint in endpoints)
            {
                sb.AppendLine($"  - {endpoint.Name}");
            }
            
        }
        catch (Exception ex)
        {
            sb.AppendLine($"Error scanning components: {ex.Message}");
        }
        
        sb.AppendLine();
    }
}
