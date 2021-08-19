using System;
using UnityEngine;

namespace ManyMoreFixes
{

    internal static class RWInputHook
    {

        public static void ApplyRWInputHK()
        {
            On.RWInput.PlayerInput += RWInput_PlayerInput;
        }

        private static Player.InputPackage RWInput_PlayerInput(On.RWInput.orig_PlayerInput orig, int playerNumber, Options options,
            RainWorldGame.SetupValues setup)
        {
            Player.InputPackage inputPackage = orig.Invoke(playerNumber, options, setup);
            if (options.controls[playerNumber].gamePad && (float)inputPackage.y < -0.45f)
            {
                if ((float)inputPackage.x < -0.45f)
                {
                    inputPackage.downDiagonal = -1;
                }
                else if ((float)inputPackage.x > 0.45f)
                {
                    inputPackage.downDiagonal = 1;
                }
            }
            return inputPackage;
        }
    }
}
