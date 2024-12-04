using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace Funnies.Commands;

public class CommandRcon
{
    public static void OnRconCommand(CCSPlayerController? caller, CommandInfo command)
    {
        if (!AdminManager.PlayerHasPermissions(caller, "@css/rcon")) return;
        
        Server.ExecuteCommand(command.ArgString);
    }
}