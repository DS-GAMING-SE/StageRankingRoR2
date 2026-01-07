using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
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
                    scores.Add(new Score
                    {
                        nameToken = "DS_GAMING_STAGE_RANKING_TIME_SCORE",
                        score = Util.ClampedNonlinearLerp(0, Config.MaxTimeScore().Value,
                    (1 - ((Run.instance.GetRunStopwatch() - Stage.instance.entryStopwatchValue) - Config.TimeForMaxTimeScore().Value) / Config.TimeForMinTimeScore().Value), 1.5f),
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
                    score = Util.ClampedNonlinearLerp(0, Config.MaxLootScore().Value, ((float)numOpenedChests) / numChests, 2f),
                    addedScoreRequirement = Config.MaxLootScore().Value
                });
            }
            #endregion
            #region Mountain Shrine Score
            if (TeleporterInteraction.instance && TeleporterInteraction.instance.shrineBonusStacks > 0)
            {
                scores.Add(new Score { nameToken = "DS_GAMING_STAGE_RANKING_MOUNTAIN_SHRINE_SCORE", 
                    score = (int)Math.Floor(Config.ScorePerMountainShrine().Value * 3 * RoR2.Util.Hyperbolic(TeleporterInteraction.instance.shrineBonusStacks / 2f)),
                    addedScoreRequirement = 0
                });
            }
            #endregion
        }

        public static void CreateRanking()
        {
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
                StageRankingPanel.CreatePanel(finalScores);
            }
        }
    }
    public struct Score : IComparable<Score>
    {
        public string nameToken;
        public int score;
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
