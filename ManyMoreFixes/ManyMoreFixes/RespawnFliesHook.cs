using System;
using System.Collections.Generic;
using System.Text;

namespace ManyMoreFixes
{
    internal static class RespawnFliesHook
    {
        public static void ApplyRespawnFliesHK()
        {
            On.ArenaBehaviors.RespawnFlies.Update += RespawnFlies_Update;
        }

        private static void RespawnFlies_Update(On.ArenaBehaviors.RespawnFlies.orig_Update orig, ArenaBehaviors.RespawnFlies self)
        {
            if (self.game.pauseMenu != null)
            {
                return;
            }
            orig(self);
        }

    }
}
