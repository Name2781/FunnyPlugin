using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace Funnies.Commands;

public class CommandInvisible
{
    public static void OnInvisibleCommand(CCSPlayerController? caller, CommandInfo command)
    {
        if (!AdminManager.PlayerHasPermissions(caller, Globals.Config.AdminPermission)) return;

        var player = Util.GetPlayerByName(command.ArgString);

        if (player != null)
        {
            if (Util.IsPlayerValid(caller))
                Util.ServerPrintToChat(caller!, $"Toggled invisiblity on {command.ArgString}");

            if (Globals.InvisiblePlayers.ContainsKey(player))
            {
                Globals.InvisiblePlayers.Remove(player);
                
                var pawn = player.PlayerPawn.Value;
                pawn!.Render = Color.FromArgb(255, pawn.Render);
                Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

                foreach (var weapon in pawn.WeaponServices!.MyWeapons)
                {
                    weapon.Value!.Render = pawn!.Render;
                    Utilities.SetStateChanged(weapon.Value, "CBaseModelEntity", "m_clrRender");
                }
            }
            else
                Globals.InvisiblePlayers.Add(player, new());
        }
        else
        {
            if (Util.IsPlayerValid(caller))
                Util.ServerPrintToChat(caller!, $"Player {command.ArgString} not found");
        }
    }
}