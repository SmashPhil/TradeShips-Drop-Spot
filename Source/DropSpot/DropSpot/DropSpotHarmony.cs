using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;

namespace DropSpot
{
    [StaticConstructorOnStartup]
    internal static class DropSpotHarmony
    {
        public static readonly Texture2D ColorWheel = ContentFinder<Texture2D>.Get("colorwheel", true);

        static DropSpotHarmony()
        {
            var harmony = new Harmony("rimworld.dropspottradeships.smashphil");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            //HarmonyInstance.DEBUG = true;

            harmony.Patch(original: AccessTools.Method(type: typeof(DropCellFinder), name: nameof(DropCellFinder.TradeDropSpot)),
                prefix: new HarmonyMethod(typeof(DropSpotHarmony),
                nameof(CustomDropSpotTradeShips)));
        }

        public static bool CustomDropSpotTradeShips(Map map, ref IntVec3 __result)
        {
            DropSpotIndicator dropSpotIndicator = map.listerBuildings.allBuildingsColonist.FirstOrDefault(x => x is DropSpotIndicator) as DropSpotIndicator;
            if (dropSpotIndicator != null && !map.roofGrid.Roofed(dropSpotIndicator.Position) && AnyAdjacentGoodDropSpot(dropSpotIndicator.Position, map, false, false))
            {
                IntVec3 dropSpot = dropSpotIndicator.Position;
                if (!DropCellFinder.TryFindDropSpotNear(dropSpot, map, out IntVec3 singleDropSpot, false, false))
                {
                    Log.Error("Could find no good TradeDropSpot near dropCenter " + dropSpot + ". Using a random standable unfogged cell.");
                    singleDropSpot = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(map) && !c.Fogged(map), map, 1000);
                }
                __result = singleDropSpot;
                return false;
            }
            return true;
        }

        private static bool AnyAdjacentGoodDropSpot(IntVec3 c, Map map, bool allowFogged, bool canRoofPunch)
        {
            return DropCellFinder.IsGoodDropSpot(c + IntVec3.North, map, allowFogged, canRoofPunch) || DropCellFinder.IsGoodDropSpot(c + IntVec3.East, map, allowFogged, canRoofPunch) || DropCellFinder.IsGoodDropSpot(c + IntVec3.South, map, allowFogged, canRoofPunch) || DropCellFinder.IsGoodDropSpot(c + IntVec3.West, map, allowFogged, canRoofPunch);
        }
    }
}
