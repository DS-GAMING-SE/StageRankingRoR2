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

            On.RoR2.Networking.NetworkPreloadManager.StartNewScenePreload_string_bool += ShowRankingUI2;

            On.RoR2.SceneExitController.SetState += DelaySceneExit;
        }

        // SceneExitController has an event for this but it's server only so it wouldn't work for a client-side mod
        public static void ShowRankingUI(On.RoR2.SceneExitController.orig_Begin orig, SceneExitController self)
        {
            StageRankingTracker.CreateRanking(self);
            orig(self);
        }
        // Attempt at covering portals, which doesn't run SceneExitController for clients
        public static void ShowRankingUI2(On.RoR2.Networking.NetworkPreloadManager.orig_StartNewScenePreload_string_bool orig, string sceneName, bool idk)
        {
            SceneDef sceneDef = SceneCatalog.FindSceneDef(sceneName);
            if (sceneDef && sceneDef.sceneType != SceneType.Invalid && sceneDef.sceneType != SceneType.Menu && sceneDef.sceneType != SceneType.Junk && sceneDef.sceneType != SceneType.Cutscene)
            {
                StageRankingTracker.CreateRanking(null);
            }
            orig(sceneName, idk);
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
