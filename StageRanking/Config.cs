using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RiskOfOptions;
using RiskOfOptions.Options;

namespace StageRanking
{
    public static class Config
    {
        #region General
        public static ConfigEntry<AnimationSpeed> AnimationDuration()
        {
            return StageRankingPlugin.instance.Config.Bind<AnimationSpeed>("General", "Animation Duration", AnimationSpeed.Default,
                "How long it will take for the UI to tally up and display the final score.\n\nWhen set to Long while you're singleplayer/host, it may increase the time it takes for the teleporter to activate to make sure the full animation has time to play.\nWhen set to Default, it will choose Long when you're singleplayer/host.");
        }
        public enum AnimationSpeed
        {
            Default,
            Short,
            Long
        }
        #endregion

        #region Time Score
        public static ConfigEntry<int> MaxTimeScore()
        {
            return StageRankingPlugin.instance.Config.Bind<int>("Time Score", "Max Time Score", 20000,
                "The maximum amount of score you can get from beating the stage quickly.");
        }
        public static ConfigEntry<int> TimeForMaxTimeScore()
        {
            return StageRankingPlugin.instance.Config.Bind<int>("Time Score", "Best Time", 300,
                "How quickly the stage must be beaten to earn the maximum time score, measured in seconds.");
        }
        public static ConfigEntry<int> TimeForMinTimeScore()
        {
            return StageRankingPlugin.instance.Config.Bind<int>("Time Score", "Worst Time", 1200,
                "How slowly the stage must be beaten to earn 0 time score, measured in seconds.");
        }
        #endregion
        #region Loot Score
        public static ConfigEntry<int> MaxLootScore()
        {
            return StageRankingPlugin.instance.Config.Bind<int>("Loot Score", "Max Loot Score", 20000, 
                "The maximum amount of score you can get from opening all chests in a stage.");
        }
        #endregion
        #region Mountain Shrine
        public static ConfigEntry<int> ScorePerMountainShrine()
        {
            return StageRankingPlugin.instance.Config.Bind<int>("Mountain Score", "Score Per Mountain Shrine", 5000,
                "The amount of score you get for the first mountain shrine stack you have. Subsequent mountain shrine stacks will gradually give less score.");
        }
        #endregion
        #region Ranking
        public static ConfigEntry<int> SRankRequirement()
        {
            return StageRankingPlugin.instance.Config.Bind<int>("Ranking", "S-Rank", 40000,
                "The score needed to achieve an S-rank.");
        }
        public static ConfigEntry<int> ARankRequirement()
        {
            return StageRankingPlugin.instance.Config.Bind<int>("Ranking", "A-Rank", 36000,
                "The score needed to achieve an A-rank.");
        }
        public static ConfigEntry<int> BRankRequirement()
        {
            return StageRankingPlugin.instance.Config.Bind<int>("Ranking", "B-Rank", 32000,
                "The score needed to achieve a B-rank.");
        }
        public static ConfigEntry<int> CRankRequirement()
        {
            return StageRankingPlugin.instance.Config.Bind<int>("Ranking", "C-Rank", 28000,
                "The score needed to achieve a C-rank.");
        }
        #endregion

        #region Risk Of Options
        public static void RiskOfOptionsSetup()
        {
            //ModSettingsManager.SetModIcon(Assets.mainAssetBundle.LoadAsset<Sprite>("texSuperBuffIcon"));
            #region General
            ModSettingsManager.AddOption(new ChoiceOption(AnimationDuration()));
            #endregion
            #region Time Score
            ModSettingsManager.AddOption(new IntFieldOption(TimeForMaxTimeScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 60 }));
            #endregion
            #region Loot Score
            ModSettingsManager.AddOption(new IntFieldOption(MaxLootScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 0 }));
            #endregion
            #region Ranking
            ModSettingsManager.AddOption(new IntFieldOption(SRankRequirement(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 0 }));
            ModSettingsManager.AddOption(new IntFieldOption(ARankRequirement(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 0 }));
            ModSettingsManager.AddOption(new IntFieldOption(BRankRequirement(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 0 }));
            ModSettingsManager.AddOption(new IntFieldOption(CRankRequirement(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 0 }));
            #endregion
        }
        #endregion

    }
}
