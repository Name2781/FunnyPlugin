using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Funnies.Commands;

namespace Funnies.Modules;

public class Invisible
{
    public static void OnPlayerTransmit(CCheckTransmitInfo info, CCSPlayerController player)
    {
        // TODO: Should store these but dont know a good way :/
        var gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First();

        if (gameRules.GameRules!.WarmupPeriod) return;
        var c4s = Utilities.FindAllEntitiesByDesignerName<CC4>("weapon_c4");

        if (c4s.Any())
        {
            var c4 = c4s.First();
            if (player!.Team != CsTeam.Terrorist && !gameRules.GameRules!.BombPlanted && !c4.IsPlantingViaUse  && !gameRules.GameRules!.BombDropped)
                info.TransmitEntities.Remove(c4);
            else
                info.TransmitEntities.Add(c4);
        }
    }

    public static void OnTick()
    {
        foreach (var invis in Globals.InvisiblePlayers)
        {
            if (!Util.IsPlayerValid(invis.Key)) continue;
            
            var alpha = 255f;

            var half = Server.CurrentTime + ((invis.Value.StartTime - Server.CurrentTime) / 2);
            if (half < Server.CurrentTime)
                alpha = invis.Value.EndTime < Server.CurrentTime ? 0 : Util.Map(Server.CurrentTime, half, invis.Value.EndTime, 255, 0);

            var progress = (int)Util.Map(alpha, 0, 255, 0, 20);
            var pawn = invis.Key.PlayerPawn.Value;
            
            invis.Key.PrintToCenterHtml(string.Concat(Enumerable.Repeat("&#9608;", progress)) + string.Concat(Enumerable.Repeat("&#9617;", 20 - progress)));

            pawn!.Render = Color.FromArgb((int)alpha, pawn.Render);
            Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

            foreach (var weapon in pawn.WeaponServices!.MyWeapons)
            {
                weapon.Value!.Render = pawn!.Render;
                Utilities.SetStateChanged(weapon.Value, "CBaseModelEntity", "m_clrRender");
            }
        }
    }

    public static HookResult OnPlayerSound(EventPlayerSound @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.InvisiblePlayers.TryGetValue(player!, out var data)) return HookResult.Continue;

        data.StartTime = Server.CurrentTime;
        data.EndTime = Server.CurrentTime + (@event.Duration * 2);

        Globals.InvisiblePlayers[player!] = data;
        return HookResult.Continue;
    }

    public static HookResult OnPlayerShoot(EventBulletImpact @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.InvisiblePlayers.TryGetValue(player!, out var data)) return HookResult.Continue;

        data.StartTime = Server.CurrentTime;
        data.EndTime = Server.CurrentTime + 0.5f;

        Globals.InvisiblePlayers[player!] = data;
        return HookResult.Continue;
    }

    public static HookResult OnPlayerStartPlant(EventBombBeginplant @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.InvisiblePlayers.TryGetValue(player!, out var data)) return HookResult.Continue;

        data.StartTime = Server.CurrentTime;
        data.EndTime = Server.CurrentTime + 1f;

        Globals.InvisiblePlayers[player!] = data;
        return HookResult.Continue;
    }

    public static HookResult OnPlayerStartDefuse(EventBombBegindefuse @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.InvisiblePlayers.TryGetValue(player!, out var data)) return HookResult.Continue;

        data.StartTime = Server.CurrentTime;
        data.EndTime = Server.CurrentTime + 1f;

        Globals.InvisiblePlayers[player!] = data;
        return HookResult.Continue;
    }

    public static HookResult OnPlayerReload(EventWeaponReload @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.InvisiblePlayers.TryGetValue(player!, out var data)) return HookResult.Continue;

        data.StartTime = Server.CurrentTime;
        data.EndTime = Server.CurrentTime + 1.5f;

        Globals.InvisiblePlayers[player!] = data;
        return HookResult.Continue;
    }

    public static HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.InvisiblePlayers.TryGetValue(player!, out var data)) return HookResult.Continue;

        data.StartTime = Server.CurrentTime;
        data.EndTime = Server.CurrentTime + 0.5f;

        Globals.InvisiblePlayers[player!] = data;
        return HookResult.Continue;
    }

    public static void Setup()
    {
        Globals.Plugin.RegisterEventHandler<EventBombBeginplant>(OnPlayerStartPlant);
        // EventPlayerShoot doesnt work so we use EventBulletImpact
        Globals.Plugin.RegisterEventHandler<EventBulletImpact>(OnPlayerShoot);
        Globals.Plugin.RegisterEventHandler<EventPlayerSound>(OnPlayerSound);
        Globals.Plugin.RegisterEventHandler<EventBombBegindefuse>(OnPlayerStartDefuse);
        Globals.Plugin.RegisterEventHandler<EventWeaponReload>(OnPlayerReload);
        Globals.Plugin.RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);

        Globals.Plugin.AddCommand("css_invisible", "Makes a player invisible", CommandInvisible.OnInvisibleCommand);
        Globals.Plugin.AddCommand("css_invis", "Makes a player invisible", CommandInvisible.OnInvisibleCommand);
    }

    public static void Cleanup()
    {
        foreach (var player in Util.GetValidPlayers())
        {
            var pawn = player.PlayerPawn.Value;

            pawn!.Render = Color.FromArgb(255, pawn.Render);
            Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

            foreach (var weapon in pawn.WeaponServices!.MyWeapons)
            {
                weapon.Value!.Render = pawn!.Render;
                Utilities.SetStateChanged(weapon.Value, "CBaseModelEntity", "m_clrRender");
            }
        }

        Globals.InvisiblePlayers.Clear();
    }
}