using R2API;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace StageRanking
{
    public class StageRankingPanel : MonoBehaviour
    {
        public static GameObject panelPrefab;
        public static SceneExitController sceneExitController;

        public static PanelMusicDef defaultPanelMusicDef = new("Play_stageranking_default_music", 7.5f, "Play_stageranking_default_music_short", 4f);
        public static PanelMusicDef[] overridePanelMusicDefs = [];

        public static bool panelActive;
        public static Action<Util.Ranking> OnStageRankingPanelEnd;
        public static void Initialize()
        {
            panelPrefab = PrefabAPI.CreateEmptyPrefab("StageRankingPanel");
            panelPrefab.AddComponent<StageRankingPanel>();
        }
        public static void CreatePanel(List<Score> scores)
        {
            CreateDebugChatMessage(scores);
            GameObject.Instantiate(panelPrefab);
        }

        public static void CreateDebugChatMessage(List<Score> scores)
        {
            Chat.AddMessage("==== Stage Ranking Panel ====");
            int totalScore = 0;
            int totalScoreRequirement = 0;
            foreach (var item in scores)
            {
                Chat.AddMessage(item.ToString());
                totalScore += item.score;
                totalScoreRequirement += item.addedScoreRequirement;
            }
            Chat.AddMessage($"<size=110%>{Util.PointsTextFormat(totalScore)} ==== </size><size=130%>{Util.GetRanking(totalScore, totalScoreRequirement)}</size>");
        }

        public IEnumerator Start()
        {
            panelActive = true;
            Log.Message("Stage Ranking Panel Start");
            yield return new WaitForSeconds(0.5f);
            PanelMusicDef panelMusicDef = GetPanelMusicDef();
            if (Config.PlayMusic().Value) { RoR2.Util.PlaySound(Util.GetIsAnimationSpeedLong() ? panelMusicDef.soundString : panelMusicDef.shortSoundString, gameObject); }
            StartCoroutine(EndPanelAfterDelay(Util.GetIsAnimationSpeedLong() ? panelMusicDef.duration : panelMusicDef.shortDuration));
        }
        IEnumerator EndPanelAfterDelay(float duration)
        {
            yield return new WaitForSeconds(duration);
            Log.Message("Stage Ranking Panel End");
            panelActive = false;
            OnStageRankingPanelEnd?.Invoke(Util.Ranking.D);
            if (sceneExitController && sceneExitController.exitState == SceneExitController.ExitState.Idle)
            {
                sceneExitController.SetState(SceneExitController.ExitState.TeleportOut);
            }
        }
        public void OnDestroy()
        {
            panelActive = false;
        }
        public static PanelMusicDef GetPanelMusicDef()
        {
            if (overridePanelMusicDefs.Length > 0)
            {
                LocalUser user = LocalUserManager.GetFirstLocalUser();
                if (user != null && user.cachedMaster)
                {
                    foreach (var item in overridePanelMusicDefs)
                    {
                        if (item.requiredSurvivorBodyName == null || item.requiredSurvivorBodyName.Contains(BodyCatalog.GetBodyName(user.cachedMaster.backupBodyIndex)))
                        {
                            return item;
                        }
                    }
                }
            }
            return defaultPanelMusicDef;
        }
    }

    public struct PanelMusicDef
    {
        public string soundString;
        public float duration;

        public string shortSoundString;
        public float shortDuration;

        public string[] requiredSurvivorBodyName;
        public PanelMusicDef(string soundString, float duration)
        {
            this.soundString = soundString;
            this.duration = duration;
            this.shortSoundString = soundString;
            this.shortDuration = duration;
            this.requiredSurvivorBodyName = null;
        }
        public PanelMusicDef(string soundString, float duration, string shortSoundString, float shortDuration)
        {
            this.soundString = soundString;
            this.duration = duration;
            this.shortSoundString = shortSoundString;
            this.shortDuration = shortDuration;
            this.requiredSurvivorBodyName = null;
        }
        public PanelMusicDef(string soundString, float duration, string shortSoundString, float shortDuration, string requiredSurvivorBodyName)
        {
            this.soundString = soundString;
            this.duration = duration;
            this.shortSoundString = shortSoundString;
            this.shortDuration = shortDuration;
            this.requiredSurvivorBodyName = [requiredSurvivorBodyName];
        }
        public PanelMusicDef(string soundString, float duration, string shortSoundString, float shortDuration, string[] requiredSurvivorBodyName)
        {
            this.soundString = soundString;
            this.duration = duration;
            this.shortSoundString = shortSoundString;
            this.shortDuration = shortDuration;
            this.requiredSurvivorBodyName = requiredSurvivorBodyName;
        }
    }
}
