using R2API;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StageRanking
{
    public class StageRankingPanel : MonoBehaviour
    {
        public static GameObject panelPrefab;

        public static PanelMusicDef defaultPanelMusicDef;
        public static PanelMusicDef[] overridePanelMusicDefs = [];

        public static bool panelActive;
        public static Action OnStageRankingPanelEnd;
        public static void Initialize()
        {
            panelPrefab = PrefabAPI.CreateEmptyPrefab("StageRankingPanel");
            panelPrefab.AddComponent<StageRankingPanel>();
            defaultPanelMusicDef = new PanelMusicDef("", 8f, "", 4.5f);
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

        public void Start()
        {
            panelActive = true;
            Log.Message("Stage Ranking Panel Start");
            PanelMusicDef panelMusicDef = GetPanelMusicDef();
            StartCoroutine(EndPanelAfterDelay(Util.GetIsAnimationSpeedLong() ? panelMusicDef.duration : panelMusicDef.shortDuration));
            RoR2.Util.PlaySound(Util.GetIsAnimationSpeedLong() ? panelMusicDef.soundString : panelMusicDef.shortSoundString, gameObject);
        }
        IEnumerator EndPanelAfterDelay(float duration)
        {
            yield return new WaitForSeconds(duration);
            Log.Message("Stage Ranking Panel End");
            panelActive = false;
            OnStageRankingPanelEnd?.Invoke();
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
                        if (item.requiredSurvivorBodyName.Length == 0 || item.requiredSurvivorBodyName.Contains(BodyCatalog.GetBodyName(user.cachedMaster.backupBodyIndex)))
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
        public PanelMusicDef(string soundString, float duration, string shortSoundString, float shortDuration)
        {
            new PanelMusicDef { soundString = soundString, duration = duration, shortSoundString = shortSoundString, shortDuration = shortDuration, requiredSurvivorBodyName = [] };
        }
        public PanelMusicDef(string soundString, float duration, string shortSoundString, float shortDuration, string requiredSurvivorBodyName)
        {
            new PanelMusicDef { soundString = soundString, duration = duration, shortSoundString = shortSoundString, shortDuration = shortDuration, requiredSurvivorBodyName = [requiredSurvivorBodyName] };
        }
        public PanelMusicDef(string soundString, float duration, string shortSoundString, float shortDuration, string[] requiredSurvivorBodyName)
        {
            new PanelMusicDef { soundString = soundString, duration = duration, shortSoundString = shortSoundString, shortDuration = shortDuration, requiredSurvivorBodyName = requiredSurvivorBodyName };
        }
    }
}
