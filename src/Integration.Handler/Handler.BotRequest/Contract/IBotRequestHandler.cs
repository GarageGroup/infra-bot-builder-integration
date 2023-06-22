using System;

namespace GarageGroup.Infra.Bot.Builder;

public interface IBotRequestHandler : IHandler<BotRequestJson, Unit>
{
}