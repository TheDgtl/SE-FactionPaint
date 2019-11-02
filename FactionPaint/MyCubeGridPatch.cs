using NLog;
using Sandbox.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Torch.Managers.PatchManager;
using Torch.Utils;
using VRage.Game;
using VRage.Game.Entity;

namespace FactionPaint
{
    class MyCubeGridPatch
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [ReflectedMethodInfo(typeof(MyCubeGrid), "ColorGridOrBlockRequestValidation")]
        private static readonly MethodInfo requestValidation;

        [ReflectedMethodInfo(typeof(MyCubeGridPatch), "ColorGridOrBlockRequestValidationPatch")]
        private static readonly MethodInfo requestValidationPatch;
        
        public static bool ColorGridOrBlockRequestValidationPatch(MyCubeGrid __instance, ref bool __result, long player)
        {
            if (player == 0L || !Sandbox.Game.Multiplayer.Sync.IsServer || __instance.BigOwners.Count == 0)
            {
                __result = true;
            }
            else
            {
                foreach (long bigOwner in __instance.BigOwners)
                {
                    MyRelationsBetweenPlayerAndBlock relationPlayerBlock = MyIDModule.GetRelationPlayerBlock(bigOwner, player, MyOwnershipShareModeEnum.Faction, MyRelationsBetweenPlayerAndBlock.Enemies, MyRelationsBetweenFactions.Enemies, MyRelationsBetweenPlayerAndBlock.FactionShare);
                    
                    if (relationPlayerBlock == MyRelationsBetweenPlayerAndBlock.Owner ||
                        relationPlayerBlock == MyRelationsBetweenPlayerAndBlock.FactionShare ||
                        relationPlayerBlock == MyRelationsBetweenPlayerAndBlock.NoOwnership)
                    {
                        __result = true;
                        break;
                    }
                }
            }
            
            // Stop the original method from running
            return false;
        }
        
        public static void Patch(PatchContext ctx)
        {
            ReflectedManager.Process(typeof(MyCubeGridPatch));

            try
            {
                ctx.GetPattern(requestValidation).Prefixes.Add(requestValidationPatch);
                Log.Info("Patching successful!");
            }
            catch (Exception e)
            {
                Log.Error(e, "Patching failed!");
            }
        }
    }
}
