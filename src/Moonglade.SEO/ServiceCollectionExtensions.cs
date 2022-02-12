using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.Net;

namespace Moonglade.SEO;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSEO(this IServiceCollection services)
    {
        services.AddHttpClient<ISeoClient, SeoClient>()
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(3,
                    retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount))));

        return services;
    }
}