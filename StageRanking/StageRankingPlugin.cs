using BepInEx;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StageRanking
{
    [BepInDependency(LanguageAPI.PluginGUID)]


    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2API.Utils.NetworkCompatibility(R2API.Utils.CompatibilityLevel.NoNeedForSync)]
    public class StageRankingPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ds_gaming";
        public const string PluginName = "StageRanking";
        public const string PluginVersion = "1.0.0";

        public static StageRankingPlugin instance;

        public void Awake()
        {
            instance = this;
            Log.Init(Logger);

            StageRankingTracker.Initialize();

            OnHooks.Initialize();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                StageRanking.Config.RiskOfOptionsSetup();
            }
        }
    }
}
