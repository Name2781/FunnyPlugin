using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace Funnies;

public static class Util
{

    public static string GetPlayerModel(CCSPlayerController player)
    {
        // This hurts
        return player.Pawn.Value!.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName;
    }

    public static bool IsPlayerValid(CCSPlayerController? plr) 
    { 
        if (plr == null) return false;
        if (plr.PlayerPawn == null) return false;
        return plr.IsValid || plr.PlayerPawn.IsValid; 
    } 

    public static List<CCSPlayerController> GetValidPlayers()
    {
        List<CCSPlayerController> validPlayers = [];
        foreach (var plr in Utilities.GetPlayers())
        {
            if (IsPlayerValid(plr))
                validPlayers.Add(plr);
        }

        return validPlayers;
    }

    public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float normalized = (value - fromMin) / (fromMax - fromMin);
        return toMin + normalized * (toMax - toMin);
    }

    public static CCSPlayerController? GetPlayerByName(string name)
    {
        return GetValidPlayers().FirstOrDefault(x => x!.PlayerName == name, null);
    }

    public static void ServerPrintToChat(CCSPlayerController player, string message)
    {
        player.PrintToChat($" {ChatColors.Green}[SERVER]{ChatColors.White} {message}");
    }
}