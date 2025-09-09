using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Serilog;
using TicketService.API;
using TicketService.API.Features.Tickets.SearchTickets;
using TicketService.API.Shared.Auth;

var builder = WebApplication.CreateBuilder(args);

// serilog config
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();

// Query Param Versioning is what I will be implementing. This showcases a few other ways of passing parameters
/*
URL versioning: https://localhost:5001/api/v1/example
Header versioning: https://localhost:5001/api/example -H 'X-Api-Version: 1'
Query parameter versioning: https://localhost:5001/api/example?api-version=1 <- What were going for.
*/
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
});

builder.Services.RegisterApplicationServices();
builder.Services.RegisterPersistenceServices();

// Log assembly information to file using reflection
TicketService.API.ServiceCollectionExtensions.LogAssemblyInfoToFile();

// add authentication and authorization to container
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddAuthorizationBuilder().AddPolicy(AuthPolicies.BeyondTrust, policy => policy.RequireRole(AuthRoles.BeyondTrust));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}
app.UseStatusCodePages();

// serilog hook into http request pipeline
app.UseSerilogRequestLogging();

// Add endpoint
var ticketSearch = app.Services.GetRequiredService<TicketSearch>();
ticketSearch.AddEndpoint(app);


app.Run();
