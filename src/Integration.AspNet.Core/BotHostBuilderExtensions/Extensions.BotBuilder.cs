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
        =>
        InnerConfigureBotBuilder(
            hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder)),
            storageResolver ?? throw new ArgumentNullException(nameof(storageResolver)));

    public static IHostBuilder ConfigureBotBuilder(this IHostBuilder hostBuilder, Func<IStorage> storageFactory)
        =>
        InnerConfigureBotBuilder(
            hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder)),
            storageFactory ?? throw new ArgumentNullException(nameof(storageFactory)));

    private static IHostBuilder InnerConfigureBotBuilder(IHostBuilder builder, Func<IServiceProvider, IStorage> storageResolver)
        =>
        builder.ConfigureServices(
            services => services.ConfigureBotBuilder(storageResolver));

    private static IHostBuilder InnerConfigureBotBuilder(IHostBuilder hostBuilder, Func<IStorage> storageFactory)
        =>
        InnerConfigureBotBuilder(
            hostBuilder, _ => storageFactory.Invoke());

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