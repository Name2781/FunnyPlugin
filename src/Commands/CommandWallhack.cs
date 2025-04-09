using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace Funnies.Commands;

public class CommandWallhack
{
    public static void OnWallhackCommand(CCSPlayerController? caller, CommandInfo command)
    {
        if (!AdminManager.PlayerHasPermissions(caller, Globals.Config.AdminPermission)) return;
        
        var player = Util.GetPlayerByName(command.ArgString);

        if (player != null)
        {
            if (Util.IsPlayerValid(caller))
                Util.ServerPrintToChat(caller!, $"Toggled wallhacks on {command.ArgString}");

            if (!Globals.Wallhackers.Remove(player))
                Globals.Wallhackers.Add(player);
        }
        else
        {
            if (Util.IsPlayerValid(caller))
                Util.ServerPrintToChat(caller!, $"Player {command.ArgString} not found");
        }
    }
}