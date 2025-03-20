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
        foreach (var entity in Globals.GlowData)
        {
            if (Globals.Wallhackers.Contains(player!))
            {
                if (!Util.IsPlayerValid(entity.Key) || !Util.IsPlayerValid(player)) continue;

                if (entity.Key.Team != player!.Team && player!.Team != CsTeam.Spectator && entity.Key.Team != CsTeam.Spectator)
                {
                    info.TransmitEntities.Add(entity.Value.ModelRelay);
                    info.TransmitEntities.Add(entity.Value.GlowEnt);
                    continue;
                }
            }

            info.TransmitEntities.Remove(entity.Value.ModelRelay);
            info.TransmitEntities.Remove(entity.Value.GlowEnt);
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
        if (!Globals.GlowData.TryGetValue(player!, out var glowData)) return HookResult.Continue;

        if (glowData.GlowEnt.IsValid)
            glowData.GlowEnt.Remove();
        if (glowData.ModelRelay.IsValid)
            glowData.ModelRelay.Remove();

        Globals.GlowData.Remove(player!);
        Globals.Wallhackers.Remove(player!);

        return HookResult.Continue;
    }

    public static HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.GlowData.TryGetValue(player!, out var glowData)) return HookResult.Continue;

        glowData.GlowEnt.Glow.GlowRange = 0;

        return HookResult.Continue;
    }

    public static HookResult OnPlayerChangeTeam(EventPlayerTeam @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Util.IsPlayerValid(player)) return HookResult.Continue;
        if (!Globals.GlowData.TryGetValue(player!, out var glowData)) return HookResult.Continue;

        Server.NextWorldUpdate(() => 
        {
            glowData.GlowEnt.SetModel(Util.GetPlayerModel(player));
            glowData.ModelRelay.SetModel(Util.GetPlayerModel(player));
        });

        return HookResult.Continue;
    }

    private static void Glow(CCSPlayerController player)
    {
        var glowEntity = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");
        var modelRelay = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");

        modelRelay!.Spawnflags = 256;
        modelRelay.Render = Color.Transparent;
        modelRelay.RenderMode = RenderMode_t.kRenderNone;
        modelRelay.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(1u << 2);
        modelRelay.SetModel(Util.GetPlayerModel(player));

        glowEntity!.Spawnflags = 256;
        glowEntity!.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(1u << 2);
        glowEntity.Render = Color.FromArgb(1, 0, 0, 0);
        glowEntity.SetModel(Util.GetPlayerModel(player));

        glowEntity.DispatchSpawn();
        modelRelay.DispatchSpawn();

        glowEntity.Glow.GlowRange = 5000;
        glowEntity.Glow.GlowRangeMin = 0;
        glowEntity.Glow.GlowColorOverride = Color.FromArgb(255, Globals.Config.R, Globals.Config.G, Globals.Config.B);
        glowEntity.Glow.GlowTeam = -1;
        glowEntity.Glow.GlowType = 3;

        modelRelay.AcceptInput("FollowEntity", player.Pawn.Value, null, "!activator");
        glowEntity.AcceptInput("FollowEntity", modelRelay, null, "!activator");

        Globals.GlowData.Remove(player);
        Globals.GlowData.Add(player, new() {
            GlowEnt = glowEntity,
            ModelRelay = modelRelay
        });
    }

    public static void Setup()
    {
        Globals.Plugin.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        Globals.Plugin.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        Globals.Plugin.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn, HookMode.Post);
        Globals.Plugin.RegisterEventHandler<EventPlayerTeam>(OnPlayerChangeTeam, HookMode.Post);

        Globals.Plugin.AddCommand("css_wh", "Gives a player walls", CommandWallhack.OnWallhackCommand);
        Globals.Plugin.AddCommand("css_wallhack", "Gives a player walls", CommandWallhack.OnWallhackCommand);
    }

    public static void Cleanup()
    {
        foreach (var entity in Globals.GlowData)
        {
            Server.NextWorldUpdate(() => 
            {
                entity.Value.GlowEnt.Remove();
                entity.Value.ModelRelay.Remove();
            });
        }

        Globals.GlowData.Clear();
        Globals.Wallhackers.Clear();
    }
}