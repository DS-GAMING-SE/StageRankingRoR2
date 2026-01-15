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

            On.RoR2.SceneExitController.SetState += DelaySceneExit;
        }

        // SceneExitController has an event for this but it's server only so it wouldn't work for a client-side mod
        public static void ShowRankingUI(On.RoR2.SceneExitController.orig_Begin orig, SceneExitController self)
        {
            StageRankingTracker.CreateRanking(self);
            orig(self);
        }
        // Prevents the SceneExitController from beginning to teleport all players away until after the StageRankingPanel is no longer active
        public static void DelaySceneExit(On.RoR2.SceneExitController.orig_SetState orig, SceneExitController self, SceneExitController.ExitState newState)
        {
            if (self.exitState != newState && newState == SceneExitController.ExitState.TeleportOut && StageRankingPanel.panelActive)
            {
                return;
            }
            orig(self, newState);
        }
    }
}
