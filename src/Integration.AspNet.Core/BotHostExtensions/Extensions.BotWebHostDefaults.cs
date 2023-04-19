using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting;

partial class BotHostExtensions
{
    public static IHostBuilder ConfigureBotWebHostDefaults(this IHostBuilder hostBuilder, Func<IServiceProvider, IBot> botResolver)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        ArgumentNullException.ThrowIfNull(botResolver);

        return hostBuilder.ConfigureWebHostDefaults(b => b.Configure(Configure));

        void Configure(IApplicationBuilder applicationBuilder)
        {
            ArgumentNullException.ThrowIfNull(applicationBuilder);
            applicationBuilder.UseWebSockets().InternalUseBot(botResolver).InternalUseStandardBotHealthCheck();
        }
    }
}