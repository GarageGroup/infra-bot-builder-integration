using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

partial class BotHostBuilderExtensions
{
    public static IHostBuilder ConfigureBotBuilder(this IHostBuilder hostBuilder, Func<IServiceProvider, IStorage> storageResolver)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        ArgumentNullException.ThrowIfNull(storageResolver);

        return hostBuilder.ConfigureServices(
            services => services.ConfigureBotBuilder(storageResolver));
    }

    public static IHostBuilder ConfigureBotBuilder(this IHostBuilder hostBuilder, Func<IStorage> storageFactory)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        ArgumentNullException.ThrowIfNull(storageFactory);

        return hostBuilder.ConfigureServices(
            services => services.ConfigureBotBuilder(InnerResolve));

        IStorage InnerResolve(IServiceProvider _)
            =>
            storageFactory.Invoke();
    }

    public static IHostBuilder ConfigureBotBuilder(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        return hostBuilder.ConfigureServices(
            services => services.ConfigureBotBuilder(InnerCreateMemoryStorage));

        static IStorage InnerCreateMemoryStorage(IServiceProvider _)
            =>
            new MemoryStorage();
    }

    private static void ConfigureBotBuilder(this IServiceCollection services, Func<IServiceProvider, IStorage> storageResolver)
        =>
        services
        .AddHttpContextAccessor()
        .AddApplicationInsightsTelemetry()
        .AddSingleton<ITelemetryInitializer, OperationCorrelationTelemetryInitializer>()
        .AddSingleton<ITelemetryInitializer, TelemetryBotIdInitializer>()
        .AddSingleton(ResolveBotTelemetryClient)
        .AddSingleton(storageResolver)
        .AddSingleton<ConversationState>(
            sp => new(sp.GetRequiredService<IStorage>()))
        .AddSingleton<UserState>(
            sp => new(sp.GetRequiredService<IStorage>()));

    private static IBotTelemetryClient ResolveBotTelemetryClient(IServiceProvider serviceProvider)
        =>
        new BotTelemetryClient(
            serviceProvider.GetRequiredService<TelemetryClient>());
}