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
            static services => services.ConfigureBotBuilder(default));
    }

    private static void ConfigureBotBuilder(this IServiceCollection services, Func<IServiceProvider, IStorage>? storageResolver)
    {
        _ = services
            .AddHttpContextAccessor()
            .AddApplicationInsightsTelemetry()
            .AddSingleton<ITelemetryInitializer, OperationCorrelationTelemetryInitializer>()
            .AddSingleton<ITelemetryInitializer, TelemetryBotIdInitializer>()
            .AddSingleton<IBotTelemetryClient, BotTelemetryClient>(ResolveBotTelemetryClient);

        if (storageResolver is not null)
        {
            _ = services.AddSingleton(storageResolver);
        }

        _ = services.AddSingleton(ResolveConversationState).AddSingleton(ResolveUserState);

        static BotTelemetryClient ResolveBotTelemetryClient(IServiceProvider serviceProvider)
            =>
            new(serviceProvider.GetRequiredService<TelemetryClient>());

        static ConversationState ResolveConversationState(IServiceProvider serviceProvider)
            =>
            new(serviceProvider.GetRequiredService<IStorage>());

        static UserState ResolveUserState(IServiceProvider serviceProvider)
            =>
            new(serviceProvider.GetRequiredService<IStorage>());
    }
}