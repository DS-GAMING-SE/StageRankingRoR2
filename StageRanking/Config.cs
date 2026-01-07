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
                "How long it will take for the UI to tally up and display the final score.\n\nWhen using the long animation while you're singleplayer/host, it may increase the time it takes for the teleporter to activate to make sure the full animation has time to play.\nWhen set to Default, it will attempt to play the long animation, but will fallback to the short one in multiplayer situations where the teleporter cannot be delayed.");
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
        public static ConfigEntry<float> SRankRequirement()
        {
            return StageRankingPlugin.instance.Config.Bind<float>("Ranking", "S-Rank", 0.9f,
                "The percentage, out of all possible score you can get, that you must earn to achieve an S-rank.");
        }
        public static ConfigEntry<float> ARankRequirement()
        {
            return StageRankingPlugin.instance.Config.Bind<float>("Ranking", "A-Rank", 0.85f,
                "The percentage, out of all possible score you can get, that you must earn to achieve an A-rank.");
        }
        public static ConfigEntry<float> BRankRequirement()
        {
            return StageRankingPlugin.instance.Config.Bind<float>("Ranking", "B-Rank", 0.75f,
                "The percentage, out of all possible score you can get, that you must earn to achieve an B-rank.");
        }
        public static ConfigEntry<float> CRankRequirement()
        {
            return StageRankingPlugin.instance.Config.Bind<float>("Ranking", "C-Rank", 0.7f,
                "The percentage, out of all possible score you can get, that you must earn to achieve an C-rank.");
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
            ModSettingsManager.AddOption(new IntFieldOption(MaxTimeScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 60 }));
            ModSettingsManager.AddOption(new IntFieldOption(TimeForMaxTimeScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 60 }));
            ModSettingsManager.AddOption(new IntFieldOption(TimeForMinTimeScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 60 }));
            #endregion
            #region Loot Score
            ModSettingsManager.AddOption(new IntFieldOption(MaxLootScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 0 }));
            #endregion
            #region Mountain Score
            ModSettingsManager.AddOption(new IntFieldOption(ScorePerMountainShrine(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 0 }));
            #endregion
            #region Ranking
            ModSettingsManager.AddOption(new FloatFieldOption(SRankRequirement(), new RiskOfOptions.OptionConfigs.FloatFieldConfig() { Min = 0, Max = 1 }));
            ModSettingsManager.AddOption(new FloatFieldOption(ARankRequirement(), new RiskOfOptions.OptionConfigs.FloatFieldConfig() { Min = 0, Max = 1 }));
            ModSettingsManager.AddOption(new FloatFieldOption(BRankRequirement(), new RiskOfOptions.OptionConfigs.FloatFieldConfig() { Min = 0, Max = 1 }));
            ModSettingsManager.AddOption(new FloatFieldOption(CRankRequirement(), new RiskOfOptions.OptionConfigs.FloatFieldConfig() { Min = 0, Max = 1 }));
            #endregion
        }
        #endregion

    }
}
