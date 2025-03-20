#if DEBUG
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace Funnies.Commands;

public class CommandDebug
{
    public static void OnDebugCommand(CCSPlayerController? caller, CommandInfo command)
    {
        foreach (var player in Util.GetValidPlayers())
        {
            if (player.IsBot)
            {
                player.SwitchTeam(CsTeam.Terrorist);
                break;
            }
        }
    }
}
#endif