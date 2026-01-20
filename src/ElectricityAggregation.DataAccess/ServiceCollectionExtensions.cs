using ElectricityAggregation.Core.Repositories;
using ElectricityAggregation.DataAccess.Db;
using ElectricityAggregation.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ElectricityAggregation.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        DatabaseMigrator.Migrate(connectionString);

        services.AddSingleton<IAggregatedElectricityRepository>(
            _ => new AggregatedElectricityRepository(connectionString));

        return services;
    }
}
