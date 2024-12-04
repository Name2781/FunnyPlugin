using CounterStrikeSharp.API.Core;
using Funnies.Models;

namespace Funnies;

public static class Globals
{
    public static List<CCSPlayerController> Wallhackers = [];
    public static Dictionary<CCSPlayerController, CDynamicProp> GlowEntities = [];

    public static Dictionary<CCSPlayerController, SoundData> InvisiblePlayers = [];

#pragma warning disable CS8618
    public static FunniesPlugin Plugin;
#pragma warning restore CS8618
}