using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.HudOverlay;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace StageRanking
{
    public class StageRankingPanel : MonoBehaviour
    {
        public static SceneExitController sceneExitController;
        private static OverlayController overlayController;

        public static List<Score> displayedScores;

        public static PanelMusicDefCombo defaultPanelMusicDef = new(
            new PanelMusicDef("Play_stageranking_default_music", 4.8f, 7.5f),
            new PanelMusicDef("Play_stageranking_default_music_short", 2.1f, 4f));
        public static PanelMusicDefCombo[] overridePanelMusicDefs = [];
        public const float panelStartPad = 0.5f;
        public const float tallyStartPadPercent = 0.2f;
        private float tallyStart;
        public const float tallyEndPad = 0.5f;
        private PanelState state;
        private PanelMusicDef panelMusicDef;
        private int totalScore;
        private int totalScoreRequirement;
        private float targetBarFill;

        private List<HGTextMeshProUGUI> ptsText;

        private float timer;

        public static bool panelActive;
        public static Action<Ranking> OnStageRankingPanelEnd;

        public Ranking ranking;

        private RankingVisual rankingVisual;
        public Image rankingForeground;
        public Image rankingBackground;
        public Image dBar;
        public Image cBar;
        public Image bBar;
        public Image aBar;
        public Transform scoreStripContainer;
        public static void CreatePanel(List<Score> scores)
        {
            if (panelActive) return;
            CreateDebugChatMessage(scores);
            displayedScores = scores;
            overlayController = HudOverlayManager.AddGlobalOverlay(new OverlayCreationParams
            {
                prefab = Assets.panelPrefab,
                childLocatorEntry = "CrosshairExtras"
            });
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
            panelMusicDef = GetPanelMusicDef();
            tallyStart = (panelMusicDef.timeUntilRankReveal - panelStartPad) * tallyStartPadPercent;
            foreach (var item in displayedScores)
            {
                totalScore += item.score;
                totalScoreRequirement += item.addedScoreRequirement;
            }
            ranking = Util.GetRanking(totalScore, totalScoreRequirement);
            CalculateTargetBarFill();
            PrepareRanking(ranking);
        }
        public void Update()
        {
            timer += Time.deltaTime;
            switch(state)
            {
                case PanelState.BeforeStart:
                    if (timer > panelStartPad)
                    {
                        SetState(PanelState.Start);
                    }
                    break;
                case PanelState.Start:
                    if (timer > panelStartPad + tallyStart)
                    {
                        SetState(PanelState.Tallying);
                    }
                    break;
                case PanelState.Tallying:
                    if (timer <= panelMusicDef.timeUntilRankReveal)
                    {
                        float fillPercent = Mathf.Clamp01((timer - panelStartPad - tallyStart) / (panelMusicDef.timeUntilRankReveal - tallyEndPad - panelStartPad - tallyStart));
                        SetBarFill(fillPercent);
                        SetPtsValues(fillPercent);
                    }
                    else if (timer > panelMusicDef.timeUntilRankReveal + panelStartPad)
                    {
                        SetState(PanelState.Reveal);
                    }
                    break;
                case PanelState.Reveal:
                    break;
            }
            if (timer > panelMusicDef.duration)
            {
                SetState(PanelState.End);
            }
        }
        public void SetState(PanelState newState)
        {
            if (state == newState) return;
            state = newState;
            switch (state)
            {
                case PanelState.BeforeStart:
                    break;
                case PanelState.Start:
                    if (Config.PlayMusic().Value) { RoR2.Util.PlaySound(panelMusicDef.soundString, gameObject); }
                    break;
                case PanelState.Tallying:
                    break;
                case PanelState.Reveal:
                    rankingBackground.color = rankingVisual.backgroundColor;
                    rankingForeground.gameObject.SetActive(true);
                    RoR2.Util.PlaySound("Play_stageranking_result", gameObject);
                    break;
                case PanelState.End:
                    Log.Message("Stage Ranking Panel End");
                    panelActive = false;
                    OnStageRankingPanelEnd?.Invoke(ranking);
                    if (sceneExitController && sceneExitController.exitState == SceneExitController.ExitState.Idle)
                    {
                        sceneExitController.SetState(SceneExitController.ExitState.TeleportOut);
                    }
                    break;
            }
        }
        public void CalculateTargetBarFill()
        {
            if (ranking == Ranking.S)
            {
                targetBarFill = 1f;
                return;
            }
            float previousRank;
            if (ranking == Ranking.D)
            {
                previousRank = 0;
            }
            else
            {
                previousRank = Util.GetScoreRequirement(ranking - 1, totalScoreRequirement);
            }
            float nextRank = Util.GetScoreRequirement(ranking + 1, totalScoreRequirement);
            targetBarFill = (float)ranking * 0.25f + (((totalScore - previousRank)/(nextRank - previousRank)) * 0.25f);
        }
        public void SetBarFill(float totalFill)
        {
            float fill = Mathf.Min(totalFill, targetBarFill);
            dBar.fillAmount = fill * 4f;
            cBar.fillAmount = (fill - 0.25f) * 4f;
            bBar.fillAmount = (fill - 0.5f) * 4f;
            aBar.fillAmount = (fill - 0.75f) * 4f;
        }
        public void SetPtsValues(float percentOfMax)
        {
            for (int i = 0; i < ptsText.Count; i++)
            {
                ptsText[i].text = Util.PointsTextFormat(Math.Min((int)(displayedScores[i].score * percentOfMax),
                    Math.Max(displayedScores[i].score, displayedScores[i].addedScoreRequirement)));
            }
        }
        public void PrepareRanking(Ranking rank)
        {
            rankingVisual = Util.GetRankingVisual(rank);
            rankingForeground.color = rankingVisual.foregroundColor;
            AssetAsyncReferenceManager<Sprite>.LoadAsset(rankingVisual.foregroundSprite, AsyncReferenceHandleUnloadType.OnSceneUnload).Completed += delegate (AsyncOperationHandle<Sprite> x)
            {
                rankingForeground.sprite = x.Result;
            };
            CreateScoreStrips();
        }
        private void CreateScoreStrips()
        {
            ptsText = new List<HGTextMeshProUGUI>();
            for (int i = 0; i < displayedScores.Count; i++)
            {
                GameObject strip = GameObject.Instantiate(Assets.scoreStripPrefab, scoreStripContainer);
                strip.transform.Find("StatNameLabel").GetComponent<HGTextMeshProUGUI>().text = Language.GetString(displayedScores[i].nameToken);
                HGTextMeshProUGUI pointsText = strip.transform.Find("PointValueLabel").GetComponent<HGTextMeshProUGUI>();
                pointsText.text = Util.PointsTextFormat(0);
                ptsText.Add(pointsText);
            }
        }
        public void OnDestroy()
        {
            HudOverlayManager.RemoveGlobalOverlay(overlayController);
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
                            return Util.GetIsAnimationSpeedLong() ? item.panelMusicDef : item.shortPanelMusicDef;
                        }
                    }
                }
            }
            return Util.GetIsAnimationSpeedLong() ? defaultPanelMusicDef.panelMusicDef : defaultPanelMusicDef.shortPanelMusicDef;
        }

        public enum PanelState
        {
            BeforeStart,
            Start,
            Tallying,
            Reveal,
            End
        }
    }
    public struct PanelMusicDefCombo
    {
        public PanelMusicDef panelMusicDef;
        public PanelMusicDef shortPanelMusicDef;

        public string[] requiredSurvivorBodyName;

        public PanelMusicDefCombo(PanelMusicDef panelMusicDef, PanelMusicDef shortPanelMusicDef)
        {
            this.panelMusicDef = panelMusicDef;
            this.shortPanelMusicDef = shortPanelMusicDef;
            this.requiredSurvivorBodyName = null;
        }
        public PanelMusicDefCombo(PanelMusicDef panelMusicDef, PanelMusicDef shortPanelMusicDef, string requiredSurvivorBodyName)
        {
            this.panelMusicDef = panelMusicDef;
            this.shortPanelMusicDef = shortPanelMusicDef;
            this.requiredSurvivorBodyName = [requiredSurvivorBodyName];
        }
        public PanelMusicDefCombo(PanelMusicDef panelMusicDef, PanelMusicDef shortPanelMusicDef, string[] requiredSurvivorBodyName)
        {
            this.panelMusicDef = panelMusicDef;
            this.shortPanelMusicDef = shortPanelMusicDef;
            this.requiredSurvivorBodyName = requiredSurvivorBodyName;
        }
    }

    public struct PanelMusicDef
    {
        public string soundString;
        public float timeUntilRankReveal;
        public float duration;
        public PanelMusicDef(string soundString, float timeUntilRankReveal, float duration)
        {
            this.soundString = soundString;
            this.timeUntilRankReveal = timeUntilRankReveal;
            this.duration = duration;
        }
    }
}
