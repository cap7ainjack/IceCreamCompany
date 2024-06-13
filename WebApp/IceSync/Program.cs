using Hangfire;
using IceSync.Application.Commands;
using IceSync.Application.DTOs;
using IceSync.Application.Integrations;
using IceSync.Application.Models.Config;
using IceSync.Application.Queries;
using IceSync.Application.Services.Background;
using IceSync.Application.Services.External;
using IceSync.Domain.Repositories;
using IceSync.Infrastructure.BackgroundServices;
using IceSync.Infrastructure.EF;
using IceSync.Infrastructure.EF.Repositories;
using IceSync.Middleware;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Add DbContext
builder.Services.AddDbContext<IceSyncDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

// Register services
builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection("ExternalApi"));

builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>()
.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<IRequestHandler<GetWorkflowsQuery, IEnumerable<WorkflowDto>>, GetWorkflowsHandler>();
builder.Services.AddScoped<IRequestHandler<RunWorkflowCommand, bool>, RunWorkflowHandler>();

// Add Repositories
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();

// Add External services
builder.Services.AddScoped<ISynchronizationService, SynchronizationService>();

builder.Services.AddMemoryCache();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

app.UseCors("AllowSpecificOrigins");

RecurringJob.AddOrUpdate<ISynchronizationService>("synchronize-workflows", x => x.SynchronizeWorkflows(),
    Cron.MinuteInterval(builder.Configuration.GetValue<int>("Hangfire:JobIntervalInMinutes")));

app.UseMiddleware<ExceptionMiddleware>();

app.Run();
