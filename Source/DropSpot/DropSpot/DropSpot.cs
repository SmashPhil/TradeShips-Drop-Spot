using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace DropSpot
{
    public class DropSpotIndicator : Building
    {
        public DropSpotIndicator()
        {
            if (Current.Game.CurrentMap != null)
            {
                if (Current.Game.CurrentMap.listerBuildings.allBuildingsColonist.Find(x => x is DropSpotIndicator) is DropSpotIndicator dropSpot)
                {
                    dropSpot.Destroy();
                    Messages.Message("DropSpotDestroyed".Translate(), MessageTypeDefOf.NeutralEvent, false);
                }
            }
        }
    }
}
