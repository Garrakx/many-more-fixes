using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ManyMoreFixes
{
    public static class RainWorldHK
    {
        public static void ApplyRainWorld()
        {
            On.RainWorld.Update += RainWorld_Update;
        }

        private static void RainWorld_Update(On.RainWorld.orig_Update orig, RainWorld self)
        {
            orig(self);
            RainWorldHK.fpsCap = MMFMod.config.fpsCap;
            if (RainWorldHK.fpsCapLast != RainWorldHK.fpsCap)
            {
                Application.targetFrameRate = RainWorldHK.fpsCap;
                if (RainWorldHK.fpsCap > 145)
                {
                    Application.targetFrameRate = -1;
                }
                Debug.Log("Target Framerate: " + Application.targetFrameRate);
            }
            RainWorldHK.fpsCapLast = RainWorldHK.fpsCap;
            if (RainWorldHK.lastQuality != MMFMod.config.quality)
            {
                self.flatIllustrations = (File.Exists(Custom.RootFolderDirectory() + "flatmode.txt") || MMFMod.config.quality == MMFMod.Quality.MEDIUM || MMFMod.config.quality == MMFMod.Quality.LOW);
                Debug.Log("MMF: Flatmode-> " + self.flatIllustrations.ToString());
            }
            RainWorldHK.lastQuality = MMFMod.config.quality;
        }

        public static int fpsCap = 144;
        public static int fpsCapLast = -20;
        public static MMFMod.Quality lastQuality = (MMFMod.Quality)5;
    }
}
