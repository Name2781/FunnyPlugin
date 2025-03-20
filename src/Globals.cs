using CounterStrikeSharp.API.Core;
using Funnies.Models;

namespace Funnies;

public static class Globals
{
    public static FunniesConfig Config { get; set; }
    public static List<CCSPlayerController> Wallhackers = [];
    public static Dictionary<CCSPlayerController, InvisibleData> GlowData = [];

    public static Dictionary<CCSPlayerController, SoundData> InvisiblePlayers = [];

#pragma warning disable CS8618
    public static FunniesPlugin Plugin;
#pragma warning restore CS8618
}