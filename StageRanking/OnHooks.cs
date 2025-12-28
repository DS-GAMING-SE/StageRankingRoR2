using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace StageRanking
{
    public static class OnHooks
    {
        public static void Initialize()
        {
            On.RoR2.SceneExitController.Begin += ShowRankingUI;
        }

        // SceneExitController has an event for this but it's server only so it wouldn't work for a client-side mod
        public static void ShowRankingUI(On.RoR2.SceneExitController.orig_Begin orig, SceneExitController self)
        {
            StageRankingTracker.CreateRanking();
            orig(self);
        }
    }
}
