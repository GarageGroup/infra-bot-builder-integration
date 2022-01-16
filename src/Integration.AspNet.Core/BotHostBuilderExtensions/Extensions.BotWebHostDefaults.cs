using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting;

partial class BotHostBuilderExtensions
{
    public static IHostBuilder ConfigureBotWebHostDefaults(this IHostBuilder hostBuilder, Func<IBotBuilder, IBotBuilder> configureBot)
    {
        _ = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
        _ = configureBot ?? throw new ArgumentNullException(nameof(configureBot));

        return hostBuilder.ConfigureWebHostDefaults(b => b.Configure(Configure));

        void Configure(IApplicationBuilder applicationBuilder)
        {
            _ = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
            InnerConfigure(applicationBuilder, configureBot);
        }
    }

    private static void InnerConfigure(IApplicationBuilder app, Func<IBotBuilder, IBotBuilder> configureBot)
        =>
        app
        .UseWebSockets()
        .UseAuthorization(
            static _ => new())
        .InternalUseBot(
            sp => sp.ResolveBot(configureBot))
        .InternalUseStandardBotHealthCheck();

    private static IBot ResolveBot(this IServiceProvider serviceProvider, Func<IBotBuilder, IBotBuilder> configureBot)
        =>
        BotBuilder.InternalCreate(
            serviceProvider,
            serviceProvider.GetRequiredService<ConversationState>(),
            serviceProvider.GetRequiredService<UserState>(),
            serviceProvider.GetRequiredService<ILoggerFactory>())
        .InnerPipe(
            configureBot)
        .Build();
}