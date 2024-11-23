using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DcmParse;

public static class DependencyInjection
{
    public static IServiceCollection AddDcmParse(this IServiceCollection services)
    {
        services.TryAddSingleton<DicomParser>();
        return services;
    }
}
