using ElectricityAggregation.Core.Aggregation;
using ElectricityAggregation.Core.Services;
using ElectricityAggregation.DataAccess;
using ElectricityAggregation.Infrastructure.Services;
using ElectricityAggregation.Worker;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL")
    ?? throw new InvalidOperationException("PostgreSQL connection string is required");

builder.Services.AddDataAccess(connectionString);

builder.Services.AddFileDownloader();
builder.Services.AddCsvParser();
builder.Services.AddElectricityDataUrlProvider(builder.Configuration);

builder.Services.AddSingleton<IElectricityAggregator, ElectricityAggregator>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IElectricityIngestionService, ElectricityIngestionService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
