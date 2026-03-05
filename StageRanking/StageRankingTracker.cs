using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

namespace StageRanking
{
    public static class StageRankingTracker
    {
        public delegate void StageRankingEventHandler(List<Score> scores);

        public static event StageRankingEventHandler OnStageRankingGatherScores;
        public static void Initialize()
        {
            OnStageRankingGatherScores += DefaultScores;
        }

        private static void DefaultScores(List<Score> scores)
        {
            #region Time Score
            if (Run.instance && Stage.instance && Stage.instance.entryStopwatchValue != Run.instance.GetRunStopwatch())
            {
                if (Config.TimeForMaxTimeScore().Value < Config.TimeForMinTimeScore().Value)
                {
                    if (Config.longStages == null) Config.UpdateLongStagesList(null, null);
                    if (Config.veryLongStages == null) Config.UpdateVeryLongStagesList(null, null);
                    float timeMult = 1f;
                    if (Config.veryLongStages.Contains(Stage.instance.sceneDef.cachedName))
                    {
                        timeMult += Config.VeryLongStageMultiplierTimeScore().Value / 100f;
                    }
                    else if (Config.longStages.Contains(Stage.instance.sceneDef.cachedName))
                    {
                        timeMult += Config.LongStageMultiplierTimeScore().Value / 100f;
                    }

                    scores.Add(new Score
                        {
                            nameToken = "DS_GAMING_STAGE_RANKING_TIME_SCORE",
                            score = Util.ClampedNonlinearLerp(0, Config.MaxTimeScore().Value,
                        (1 - ((Run.instance.GetRunStopwatch() - Stage.instance.entryStopwatchValue) - (Config.TimeForMaxTimeScore().Value) * timeMult)/ (Config.TimeForMinTimeScore().Value * timeMult)), 0.7f),
                            addedScoreRequirement = Config.MaxTimeScore().Value
                        });
                }
                else
                {
                    Log.Error("Time Score's Worst Time cannot be lower than Best Time");
                }
            }
            #endregion
            #region Loot Score
            // This sucks
            // Shrines of Chance aren't counted since those don't have an instance list. I'd have to comb through every purchaseinteraction on the stage and filter out whats what, then..
            // ..add separate custom support for tracking whether shrines of chance empty since they function differently from chests, all while being entirely client side and still..
            // ..keeping track even if the interactable is deleted by Drifter. I don't think there's a sane way to do this
            // Probably missing other interactables that would make sense, but whatever. The score isn't linear, so it's hard for people to tell exactly how many interactables were..
            // ..counted based on the score alone, so no one will notice how scuffed this is, probably :)
            int numChests = 0;
            int numOpenedChests = 0;
            foreach (var item in InstanceTracker.GetInstancesList<ChestBehavior>())
            {
                numChests++;
                if (item.isChestOpened) numOpenedChests++;
            }
            if (numChests > 0)
            {
                scores.Add(new Score
                {
                    nameToken = "DS_GAMING_STAGE_RANKING_LOOT_SCORE",
                    score = Util.ClampedNonlinearLerp(0, Config.MaxLootScore().Value, ((float)numOpenedChests) / numChests, 1.5f),
                    addedScoreRequirement = Config.MaxLootScore().Value
                });
            }
            #endregion
            #region Mountain Shrine Score
            if (TeleporterInteraction.instance && TeleporterInteraction.instance.shrineBonusStacks > 0 || TeleporterInteraction.instance.NetworkshowAccessCodesIndicator)
            {
                scores.Add(new Score { nameToken = "DS_GAMING_STAGE_RANKING_MOUNTAIN_SHRINE_SCORE", 
                    score = (int)Math.Floor(Config.ScorePerMountainShrine().Value * 3 * RoR2.Util.Hyperbolic((TeleporterInteraction.instance.shrineBonusStacks + (TeleporterInteraction.instance.NetworkshowAccessCodesIndicator ? 1 : 0))/ 2f)),
                    addedScoreRequirement = 0
                });
            }
            #endregion
        }

        public static void CreateRanking(SceneExitController sceneExitController)
        {
            if (StageRankingPanel.panelActive) return;
            List<Score> finalScores = new();
            if (OnStageRankingGatherScores != null)
            {
                foreach (StageRankingEventHandler @event in OnStageRankingGatherScores.GetInvocationList().Cast<StageRankingEventHandler>())
                {
                    try
                    {
                        @event(finalScores);
                    }
                    catch (Exception e)
                    {
                        Log.Error(
                            $"Exception thrown by : {@event.Method.DeclaringType.Name}.{@event.Method.Name}:\n{e}");
                    }
                }
            }
            if (finalScores.Count > 0)
            {
                StageRankingPanel.sceneExitController = NetworkServer.active ? sceneExitController : null;
                StageRankingPanel.CreatePanel(finalScores);
            }
        }
    }
    public struct Score : IComparable<Score>
    {
        /// <summary>
        /// Name of the score
        /// </summary>
        public string nameToken;
        /// <summary>
        /// Name of the score
        /// </summary>
        public int score;
        /// <summary>
        /// How much this score raises the overall score requirement for reaching ranks. 
        /// <para>
        /// Scores that should be required to be earned to get a good rank, like Time and Loot score, add to the score requirement. Scores that are optional and should only benefit the player if accomplished, like Mountain Shrine score, should not
        /// </para>
        /// </summary>
        public int addedScoreRequirement;

        public int CompareTo(Score other)
        {
            int str = this.nameToken.CompareTo(other.nameToken);
            if (str == 0)
            {
                return this.score.CompareTo(other.score);
            }
            return str;
        }

        public override string ToString()
        {
            return Util.ScoreTextFormat(Language.GetString(this.nameToken, Language.currentLanguageName), this.score);
        }
    }
}
