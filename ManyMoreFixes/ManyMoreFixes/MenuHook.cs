using Menu;
using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ManyMoreFixes
{
    internal static class MenuHK
    {
        public static void ApplyMenuHK()
        {
            On.Menu.SlugcatSelectMenu.ctor += SlugcatSelectMenu_ctor;
            On.Menu.SleepAndDeathScreen.Update += SleepAndDeathScreen_Update;
        }

        public static int sceneProgress = 0;
        private static void SlugcatSelectMenu_ctor(On.Menu.SlugcatSelectMenu.orig_ctor orig, Menu.SlugcatSelectMenu self, ProcessManager manager)
        {
            orig(self, manager);
            if (MMFMod.config.quality != MMFMod.Quality.HIGH)
            {
                self.rainEffect = null;
            }
        }

        // CURSED
        private static void SleepAndDeathScreen_Update(On.Menu.SleepAndDeathScreen.orig_Update orig, Menu.SleepAndDeathScreen self)
        {
            if (MMFMod.config.quality == MMFMod.Quality.HIGH)
            {
                orig(self);
                return;
            }
            if (self.starvedWarningCounter >= 0)
            {
                self.starvedWarningCounter++;
            }
            IntPtr functionPointer = typeof(KarmaLadderScreen).GetMethod("Update").MethodHandle.GetFunctionPointer();
            ((Action)Activator.CreateInstance(typeof(Action), new object[]
            {
                self,
                functionPointer
            }))();
            if (self.exitButton != null)
            {
                self.exitButton.buttonBehav.greyedOut = self.ButtonsGreyedOut;
            }
            if (self.passageButton != null)
            {
                self.passageButton.buttonBehav.greyedOut = (self.ButtonsGreyedOut || self.goalMalnourished);
                self.passageButton.black = Mathf.Max(0f, self.passageButton.black - 0.0125f);
            }
            if (self.endGameSceneCounter >= 0)
            {
                self.endGameSceneCounter++;
                if (self.endGameSceneCounter > 140)
                {
                    self.manager.RequestMainProcessSwitch((ProcessManager.ProcessID)15);
                }
            }
            if (self.RevealMap)
            {
                self.fadeOutIllustration = Custom.LerpAndTick(self.fadeOutIllustration, 1f, 0.02f, 0.025f);
                return;
            }
            self.fadeOutIllustration = Custom.LerpAndTick(self.fadeOutIllustration, 0f, 0.02f, 0.025f);
        }
    }
}
