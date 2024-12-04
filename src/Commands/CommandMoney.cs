using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace Funnies.Commands;

public class CommandMoney
{
    public static void OnMoneyCommand(CCSPlayerController? caller, CommandInfo command)
    {
        if (!AdminManager.PlayerHasPermissions(caller, "@css/generic")) return;
        
        int index = command.ArgString.IndexOf(' ');
        int money = int.Parse(command.ArgString[..index]);
        var name = command.ArgString[(index + 1)..];
        var player = Util.GetPlayerByName(name);

        if (player != null)
        {
            if (Util.IsPlayerValid(caller))
                Util.ServerPrintToChat(caller!, $"Set {name}'s money to ${money}");

            player.InGameMoneyServices!.Account = money;
            Utilities.SetStateChanged(player, "CCSPlayerController", "m_pInGameMoneyServices");
        }
        else
        {
            if (Util.IsPlayerValid(caller))
                Util.ServerPrintToChat(caller!, $"Player {name} not found");
        }
    }
}