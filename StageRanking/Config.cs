using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RiskOfOptions;
using RiskOfOptions.Options;
using System.Linq;

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
        public static ConfigEntry<bool> PlayMusic()
        {
            return StageRankingPlugin.instance.Config.Bind<bool>("General", "Enable Music", true,
                "Whether the ranking screen will play music.");
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
        public static ConfigEntry<string> LongStagesTimeScore()
        {
            return StageRankingPlugin.instance.Config.Bind<string>("Time Score", "Long Stages", "rootjungle, dampcavesimple, shipgraveyard, repurposedcrater",
                "Stages that are expected to take longer than normal, giving you more time to get a good rank. By default, this includes all stage 4s.\n\nYou must put in the internal name of the stage. Use list_scene in the console to see the internal names of stages.");
        }
        public static string[] longStages;
        public static ConfigEntry<float> LongStageMultiplierTimeScore()
        {
            return StageRankingPlugin.instance.Config.Bind<float>("Time Score", "Long Multiplier", 0.25f,
                "A multiplier for how much more time you have when on long stages.");
        }
        public static ConfigEntry<string> VeryLongStagesTimeScore()
        {
            return StageRankingPlugin.instance.Config.Bind<string>("Time Score", "Very Long Stages", "conduitcanyon, moon, moon2",
                "Stages that are expected to take a lot longer than normal, giving you more time to get a good rank. By default, this includes Conduit Canyon and Commencement.\n\nYou must put in the internal name of the stage. Use list_scene in the console to see the internal names of stages.");
        }
        public static string[] veryLongStages;
        public static ConfigEntry<float> VeryLongStageMultiplierTimeScore()
        {
            return StageRankingPlugin.instance.Config.Bind<float>("Time Score", "Very Long Multiplier", 0.5f,
                "A multiplier for how much more time you have when on very long stages.");
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
            ModSettingsManager.AddOption(new CheckBoxOption(PlayMusic()));
            #endregion
            #region Time Score
            ModSettingsManager.AddOption(new IntFieldOption(MaxTimeScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 60 }));
            ModSettingsManager.AddOption(new IntFieldOption(TimeForMaxTimeScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 60 }));
            ModSettingsManager.AddOption(new IntFieldOption(TimeForMinTimeScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 60 }));
            ModSettingsManager.AddOption(new StringInputFieldOption(LongStagesTimeScore()));
            LongStagesTimeScore().SettingChanged += UpdateLongStagesList;
            ModSettingsManager.AddOption(new SliderOption(LongStageMultiplierTimeScore(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = 0, max = 1 }));
            ModSettingsManager.AddOption(new StringInputFieldOption(VeryLongStagesTimeScore()));
            ModSettingsManager.AddOption(new SliderOption(VeryLongStageMultiplierTimeScore(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = 0, max = 1 }));

            #endregion
            #region Loot Score
            ModSettingsManager.AddOption(new IntFieldOption(MaxLootScore(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 0 }));
            #endregion
            #region Mountain Score
            ModSettingsManager.AddOption(new IntFieldOption(ScorePerMountainShrine(), new RiskOfOptions.OptionConfigs.IntFieldConfig() { Min = 0 }));
            #endregion
            #region Ranking
            ModSettingsManager.AddOption(new SliderOption(SRankRequirement(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = 0, max = 1 }));
            ModSettingsManager.AddOption(new SliderOption(ARankRequirement(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = 0, max = 1 }));
            ModSettingsManager.AddOption(new SliderOption(BRankRequirement(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = 0, max = 1 }));
            ModSettingsManager.AddOption(new SliderOption(CRankRequirement(), new RiskOfOptions.OptionConfigs.SliderConfig() { min = 0, max = 1 }));
            #endregion
        }
        #endregion

        public static void UpdateLongStagesList(object sender, EventArgs args)
        {
            longStages = Config.LongStagesTimeScore().Value.Split(", ").ToArray();
        }
        public static void UpdateVeryLongStagesList(object sender, EventArgs args)
        {
            veryLongStages = Config.VeryLongStagesTimeScore().Value.Split(", ").ToArray();
        }
    }
}
