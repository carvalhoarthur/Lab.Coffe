using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Lab.Coffe.API.Middleware;
using Lab.Coffe.Application.Mappings;
using Lab.Coffe.Application.Validators;
using Lab.Coffe.Infrastructure;
using Lab.Coffe.Infrastructure.Logging;
using MediatR;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
SerilogExtensions.ConfigureSerilog(builder.Configuration, builder.Environment.EnvironmentName);
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration - será gerado automaticamente com informações dos controllers
builder.Services.AddSwaggerGen(c =>
{
    // Documento Swagger v1 - será gerado automaticamente com informações básicas
    // O Swashbuckle gera automaticamente os metadados da API
    // Include XML comments if available
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Lab.Coffe.Application.UseCases.Coffee.GetAllCoffeesQuery).Assembly));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCoffeeRequestValidator>();

// Infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "self" });

var app = builder.Build();

// Configure the HTTP request pipeline

// Swagger - habilitar em Development e para debug local
// Em produção, considere adicionar autenticação ou desabilitar
var isDevelopment = app.Environment.IsDevelopment() || 
                    app.Environment.EnvironmentName == "Development" ||
                    builder.Configuration.GetValue<bool>("EnableSwagger", false);

if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab.Coffe API v1");
        c.RoutePrefix = "swagger"; // Swagger UI em /swagger para corresponder ao launchUrl
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Correlation ID middleware (for observability)
app.UseMiddleware<CorrelationIdMiddleware>();

// Exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

// Health checks endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("self")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});

app.Run();
