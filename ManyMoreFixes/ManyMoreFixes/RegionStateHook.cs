using System;
using System.Collections.Generic;
using System.Text;

namespace ManyMoreFixes
{
    internal static class RegionStateHook
    {
        public static void ApplyRegionState()
        {
            On.RegionState.RainCycleTick += RegionState_RainCycleTick;
        }

        private static void RegionState_RainCycleTick(On.RegionState.orig_RainCycleTick orig, RegionState self, int ticks, int foodRepBonus)
        {
            orig(self, ticks, foodRepBonus);
            if (ticks > 0)
            {
                for (int i = 0; i < self.world.NumberOfRooms; i++)
                {
                    AbstractRoom abstractRoom = self.world.GetAbstractRoom(self.world.firstRoomIndex + i);
                    for (int j = 0; j < abstractRoom.entities.Count; j++)
                    {
                        if ((!(abstractRoom.entities[j] is AbstractSpear) || !(abstractRoom.entities[j] as AbstractSpear).stuckInWall) &&
                            abstractRoom.entities[j] is AbstractCreature && (abstractRoom.entities[j] as AbstractCreature).state.socialMemory == null)
                        {
                            (abstractRoom.entities[j] as AbstractCreature).state.CycleTick();
                        }
                    }
                    for (int k = 0; k < abstractRoom.entitiesInDens.Count; k++)
                    {
                        if (abstractRoom.entitiesInDens[k] is AbstractCreature)
                        {
                            (abstractRoom.entitiesInDens[k] as AbstractCreature).state.CycleTick();
                        }
                    }
                }
            }
        }
    }
}
