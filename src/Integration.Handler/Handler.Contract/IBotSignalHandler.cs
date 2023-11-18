using System;

namespace GarageGroup.Infra.Bot.Builder;

public interface IBotSignalHandler : IHandler<BotRequestJson, Unit>
{
}