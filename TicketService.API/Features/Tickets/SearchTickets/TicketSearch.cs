using Asp.Versioning;
using TicketService.API.Shared.Auth;
using TicketService.API.Shared.Domain.Models;
using TicketService.API.Shared.Networking;

namespace TicketService.API.Features.Tickets.SearchTickets;

public sealed class TicketSearch
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        List<Ticket> tickets = [
        new Ticket("I am locked out of PC") {
        Description = "Cannot login to my workstation",
        CreatedByUserId = "user123",
        Status = "Open",
        Priority = "High",
        Category = "Access"
        },
        new Ticket("Printer not working") {
            Description = "Office printer showing error message",
            CreatedByUserId = "user456",
            Status = "In Progress",
            Priority = "Medium",
            AssignedToUserId = "tech789",
            Category = "Hardware"
        },
        new Ticket("Email sync issues") {
            Description = "Outlook not syncing with server",
            CreatedByUserId = "user789",
            Status = "Open",
            Priority = "Low",
            Category = "Software"
        },
        new Ticket("Network connectivity problems") {
            Description = "Cannot connect to company network",
            CreatedByUserId = "user101",
            Status = "Resolved",
            Priority = "High",
            AssignedToUserId = "tech789",
            Category = "Network"
        },
        new Ticket("Software license expired") {
            Description = "Adobe Creative Suite license needs renewal",
            CreatedByUserId = "user202",
            Status = "Open",
            Priority = "Medium",
            Category = "Licensing"
        },
        new Ticket("Monitor display issues") {
            Description = "Screen flickering and showing artifacts",
            CreatedByUserId = "user303",
            Status = "In Progress",
            Priority = "Medium",
            AssignedToUserId = "tech456",
            Category = "Hardware"
        },
        new Ticket("Password reset request") {
            Description = "Need to reset my account password",
            CreatedByUserId = "user404",
            Status = "Open",
            Priority = "High",
            Category = "Access"
        },
        new Ticket("VPN connection failed") {
            Description = "Cannot establish VPN connection from home",
            CreatedByUserId = "user505",
            Status = "Resolved",
            Priority = "Medium",
            AssignedToUserId = "tech789",
            Category = "Network"
        },
        new Ticket("Meeting room projector not working") {
            Description = "Conference room A projector displays no signal",
            CreatedByUserId = "user606",
            Status = "Open",
            Priority = "Medium",
            Category = "Hardware"
        }];

        var ticketsVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .HasApiVersion(new ApiVersion(2, 0))
            .Build();

        // Version 1.0 - Basic search functionality
        app.MapGet("/tickets", (string? searchFor,
        ILoggerFactory loggerFactory,
        ITicketSearchApiClient ticketSearchApiClient,
        CancellationToken cancellationToken) =>
        {
            var logger = loggerFactory.CreateLogger(typeof(TicketSearch));
            logger.LogInformation("Ticket Search API v1.0 Called");

            var filteredTickets = tickets.Where(t =>
            searchFor == null ||
            t.Title.Contains(searchFor, StringComparison.OrdinalIgnoreCase) ||
            (t.Description != null && t.Description.Contains(searchFor, StringComparison.OrdinalIgnoreCase)) ||
            t.Category.Contains(searchFor, StringComparison.OrdinalIgnoreCase));

            return Results.Ok(filteredTickets);
        })
        .WithApiVersionSet(ticketsVersionSet)
        .MapToApiVersion(1, 0)
        .RequireAuthorization(AuthPolicies.BeyondTrust);

        // Version 2.0 - Enhanced search with additional filtering options
        app.MapGet("/tickets", (string? searchFor, 
            string? status, 
            string? priority, 
            string? category,
            ILoggerFactory loggerFactory,
            ITicketSearchApiClient ticketSearchApiClient,
            CancellationToken cancellationToken) =>
        {
            var logger = loggerFactory.CreateLogger(typeof(TicketSearch));
            logger.LogInformation("Ticket Search API v2.0 Called");

            var filteredTickets = tickets.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchFor))
            {
                filteredTickets = filteredTickets.Where(t =>
                    t.Title.Contains(searchFor, StringComparison.OrdinalIgnoreCase) ||
                    (t.Description != null && t.Description.Contains(searchFor, StringComparison.OrdinalIgnoreCase)) ||
                    t.Category.Contains(searchFor, StringComparison.OrdinalIgnoreCase));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                filteredTickets = filteredTickets.Where(t => 
                    t.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            // Apply priority filter
            if (!string.IsNullOrEmpty(priority))
            {
                filteredTickets = filteredTickets.Where(t => 
                    t.Priority.Equals(priority, StringComparison.OrdinalIgnoreCase));
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(category))
            {
                filteredTickets = filteredTickets.Where(t => 
                    t.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            return Results.Ok(filteredTickets.ToList());
        })
        .WithApiVersionSet(ticketsVersionSet)
        .MapToApiVersion(2, 0)
        .RequireAuthorization(AuthPolicies.BeyondTrust);
        
    }
}

