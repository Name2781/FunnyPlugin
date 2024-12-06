using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using Funnies.Commands;

namespace Funnies.Modules;

public class Wallhack
{
    public static void OnPlayerTransmit(CCheckTransmitInfo info, CCSPlayerController player)
    {
        foreach (var entity in Globals.GlowEntities)
        {
            if (Globals.Wallhackers.Contains(player!))
            {
                if (entity.Key.Team != player!.Team)
                {
                    info.TransmitEntities.Add(entity.Value);
                    continue;
                }
            }

            info.TransmitEntities.Remove(entity.Value);
        }
    }

    public static HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (player!.Team < CsTeam.Terrorist) return HookResult.Continue; // if player isnt on a team

        Glow(player!);

        return HookResult.Continue;
    }

    public static HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.GlowEntities.TryGetValue(player!, out var glowEntity)) return HookResult.Continue;

        if (glowEntity.IsValid)
            glowEntity.Remove();

        Globals.GlowEntities.Remove(player!);
        Globals.Wallhackers.Remove(player!);

        return HookResult.Continue;
    }

    public static HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.GlowEntities.TryGetValue(player!, out var glowEntity)) return HookResult.Continue;

        glowEntity.Glow.GlowRange = 0;
        // Its dumb to call this again but the server crashes if i do anything else
        glowEntity.DispatchSpawn();

        return HookResult.Continue;
    }

    private static void Glow(CCSPlayerController player)
    {
        var glowEntity = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");
        var modelRelay = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");

        modelRelay!.Spawnflags = 256;
        modelRelay.Render = Color.Transparent;
        modelRelay.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags = (uint)(modelRelay.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags & ~(1 << 2));
        modelRelay.SetModel(Util.GetPlayerModel(player));

        // https://github.com/exkludera/cs2-glowing-entities/blob/main/src/main.cs
        glowEntity!.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags = (uint)(glowEntity.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags & ~(1 << 2));

        glowEntity.SetModel(Util.GetPlayerModel(player));

        glowEntity.DispatchSpawn();
        modelRelay.DispatchSpawn();

        glowEntity.Glow.GlowRange = 5000;
        glowEntity.Glow.GlowRangeMin = 0;
        glowEntity.Glow.GlowColorOverride = Color.FromArgb(255, 171, 75, 209);
        glowEntity.Glow.GlowTeam = -1;
        glowEntity.Glow.GlowType = 3;

        modelRelay.AcceptInput("FollowEntity", player.Pawn.Value, null, "!activator");
        glowEntity.AcceptInput("FollowEntity", modelRelay, null, "!activator");

        Globals.GlowEntities.Remove(player);
        Globals.GlowEntities.Add(player, glowEntity);
    }

    public static void Setup()
    {
        Globals.Plugin.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        Globals.Plugin.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        Globals.Plugin.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn, HookMode.Post);

        Globals.Plugin.AddCommand("css_wh", "Gives a player walls", CommandWallhack.OnWallhackCommand);
        Globals.Plugin.AddCommand("css_wallhack", "Gives a player walls", CommandWallhack.OnWallhackCommand);
    }

    public static void Cleanup()
    {
        foreach (var entity in Globals.GlowEntities)
        {
            var glowEntity = entity.Value;
            if (glowEntity.IsValid)
                glowEntity.Remove();
        }

        Globals.GlowEntities.Clear();
        Globals.Wallhackers.Clear();
    }
}