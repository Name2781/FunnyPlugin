#if DEBUG
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace Funnies.Commands;

public class CommandDebug
{
    public static void OnDebugCommand(CCSPlayerController? caller, CommandInfo command)
    {
        foreach (var ent in Utilities.GetAllEntities())
        {
            try
            {
                var name = ent.As<CBaseModelEntity>().CBodyComponent.SceneNode.GetSkeletonInstance().ModelState.ModelName;
                Console.WriteLine($"{ent.DesignerName} | {name}");
            }
            catch {};
        }
    }
}
#endif