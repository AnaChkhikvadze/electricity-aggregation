using ElectricityAggregation.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace ElectricityAggregation.Infrastructure.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileDownloader(this IServiceCollection services)
    {
        services
            .AddHttpClient<IFileDownloader, FileDownloader>()
            .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    public static IServiceCollection AddCsvParser(this IServiceCollection services)
    {
        services.AddSingleton<ICsvParser, CsvParser>();
        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
