using DcmParse.Values;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DcmParse;

public static class DependencyInjection
{
    public static IServiceCollection AddDcmParse(this IServiceCollection services)
    {
        services.TryAddSingleton<IDicomParser, DicomParser>();

        // Value parsing
        services.TryAddSingleton<DicomValueParser>();

        services.TryAddSingleton<AEParser>();
        services.TryAddSingleton<ASParser>();
        services.TryAddSingleton<ATParser>();
        services.TryAddSingleton<CSParser>();
        services.TryAddSingleton<DAParser>();
        services.TryAddSingleton<DSParser>();
        services.TryAddSingleton<DTParser>();
        services.TryAddSingleton<FDParser>();
        services.TryAddSingleton<FLParser>();
        services.TryAddSingleton<ISParser>();
        services.TryAddSingleton<LOParser>();
        services.TryAddSingleton<LTParser>();
        services.TryAddSingleton<PNParser>();
        services.TryAddSingleton<SHParser>();
        services.TryAddSingleton<SLParser>();
        services.TryAddSingleton<SSParser>();
        services.TryAddSingleton<STParser>();
        services.TryAddSingleton<SVParser>();
        services.TryAddSingleton<TMParser>();
        services.TryAddSingleton<UCParser>();
        services.TryAddSingleton<UIParser>();
        services.TryAddSingleton<ULParser>();
        services.TryAddSingleton<USParser>();
        services.TryAddSingleton<UTParser>();
        services.TryAddSingleton<UVParser>();
        return services;
    }
}
