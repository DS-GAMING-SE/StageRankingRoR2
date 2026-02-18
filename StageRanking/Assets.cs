using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using RoR2.ContentManagement;
using RoR2BepInExPack;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using RoR2.UI;
using LeTai.Asset.TranslucentImage;

namespace StageRanking
{
    internal static class Assets
    {
        public static string AddressablesDirectory;

        public static GameObject panelPrefab;
        public static GameObject scoreStripPrefab;

        public static RankingVisual dRanking;
        public static RankingVisual cRanking;
        public static RankingVisual bRanking;
        public static RankingVisual aRanking;
        public static RankingVisual sRanking;

        #region Addressable GUID
        public static AssetReferenceT<GameObject> panelAsset = new AssetReferenceT<GameObject>("b1d1223b357e72f4da52ba905c9d3ca9");
        public static AssetReferenceT<GameObject> scoreStripAsset = new AssetReferenceT<GameObject>("1b50e414ba40d494caff8401e6e2438f");
        public static AssetReferenceSprite dRankingSprite = new AssetReferenceSprite("eeab231232620244a83e8fff99f26271");
        public static AssetReferenceSprite cRankingSprite = new AssetReferenceSprite("88bd72cc3cf158f45a92008d679ece3a");
        public static AssetReferenceSprite bRankingSprite = new AssetReferenceSprite("b5a33a91a6f7a9845913b0fcc0d35913");
        public static AssetReferenceSprite aRankingSprite = new AssetReferenceSprite("ee8acb6cef497bd4d919e1ae27fb891e");
        public static AssetReferenceSprite sRankingSprite = new AssetReferenceSprite("2d827acc23a9baa4bbc476a98c26c8cd");
        #endregion
        internal static void Initialize()
        {
            AddressablesDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(StageRankingPlugin.instance.Info.Location), "Addressables");
            Addressables.LoadContentCatalogAsync(System.IO.Path.Combine(AddressablesDirectory, "catalog.json")).WaitForCompletion();

            panelPrefab = Addressables.LoadAssetAsync<GameObject>(panelAsset).WaitForCompletion();
            StageRankingPanel panelComponent = panelPrefab.AddComponent<StageRankingPanel>();

            panelComponent.rankingBackground = panelPrefab.transform.Find("RankingBackground").GetComponent<Image>();
            panelComponent.rankingForeground = panelComponent.rankingBackground.transform.GetChild(0).GetComponent<Image>();
            Image scoreHeader = panelPrefab.transform.Find("ScoreContainer/ScoreHeader").GetComponent<Image>();
            TranslucentImage blurPanel = panelPrefab.transform.Find("ScoreContainer/BlurPanel").gameObject.AddComponent<TranslucentImage>();
            blurPanel.color = Color.black;
            Image scoreBody = panelPrefab.transform.Find("ScoreContainer/ScoreBody").GetComponent<Image>();

            Transform barTransform = panelPrefab.transform.Find("ScoreContainer/ScoreBody/BarPanel");
            Image barPanel = barTransform.GetComponent<Image>();
            Image barInterior = barTransform.GetChild(0).GetComponent<Image>();
            panelComponent.dBar = barTransform.GetChild(0).GetChild(0).GetComponent<Image>();
            panelComponent.cBar = barTransform.GetChild(0).GetChild(1).GetComponent<Image>();
            panelComponent.bBar = barTransform.GetChild(0).GetChild(2).GetComponent<Image>();
            panelComponent.aBar = barTransform.GetChild(0).GetChild(3).GetComponent<Image>();

            // Dissect GameEndReportPanel and steal all of its images and colors and materials
            AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_UI.GameEndReportPanel_prefab)).Completed += delegate (AsyncOperationHandle<GameObject> x)
            {
                panelComponent.rankingBackground.sprite = x.Result.transform.Find("SafeArea (JUICED)/HeaderArea/ResultArea/ResultIconBackground").GetComponent<Image>().sprite;
                Transform container = x.Result.transform.Find("SafeArea (JUICED)/BodyArea/StatsAndChatArea/StatsContainer");
                scoreHeader.sprite = container.Find("Stats And Player Nav/Stats Header").GetComponent<Image>().sprite;
                barPanel.sprite = scoreHeader.sprite;
                GameObject scoreHeaderTextObject = GameObject.Instantiate(container.Find("Stats And Player Nav/Stats Header/InfoLabel").gameObject);
                scoreHeaderTextObject.GetComponent<LanguageTextMeshController>()._token = "DS_GAMING_STAGE_RANKING_SCORE";
                scoreHeaderTextObject.transform.SetParent(scoreHeader.transform);
                scoreHeaderTextObject.transform.localPosition = Vector3.zero;
                scoreHeaderTextObject.transform.localEulerAngles = Vector3.zero;
                scoreHeaderTextObject.transform.localScale = Vector3.one;
                HGTextMeshProUGUI scoreHeaderText = scoreHeaderTextObject.GetComponent<HGTextMeshProUGUI>();
                scoreHeaderText.text = "Score";
                scoreHeaderText.fontSizeMin = 12;
                scoreHeaderText.fontSize = 18;
                scoreHeaderText.fontSizeMax = 24;
                panelPrefab.transform.Find("ScoreContainer/BorderImage").GetComponent<Image>().sprite = container.Find("BorderImage").GetComponent<Image>().sprite;
                scoreBody.sprite = container.Find("Stats Body").GetComponent<Image>().sprite;
                barInterior.sprite = scoreBody.sprite;
                TranslucentImage originalBlur = container.Find("BlurPanel").GetComponent<TranslucentImage>();
                blurPanel.material = originalBlur.material;
                blurPanel.sprite = originalBlur.sprite;
            };



            scoreStripPrefab = Addressables.LoadAssetAsync<GameObject>(scoreStripAsset).WaitForCompletion();

            Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_UI.matUIIconLoss_mat).Completed += delegate (AsyncOperationHandle<Material> x)
            {
                panelComponent.rankingBackground.material = x.Result;
                panelComponent.rankingBackground.transform.GetChild(0).GetComponent<Image>().material = x.Result;
            };
            Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_UI.texUINonsegmentedHealthbar_png_texUINonsegmentedHealthbar_).Completed += delegate (AsyncOperationHandle<Sprite> x)
            {
                panelComponent.dBar.sprite = x.Result;
                panelComponent.cBar.sprite = x.Result;
                panelComponent.bBar.sprite = x.Result;
                panelComponent.aBar.sprite = x.Result;
            };

            dRanking = new RankingVisual { foregroundColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier1ItemDark), 
                foregroundSprite = dRankingSprite,
                backgroundColor = new Color(0.4f, 0.4f, 0.5f) };
            cRanking = new RankingVisual { foregroundColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier1Item),
                foregroundSprite = cRankingSprite,
                backgroundColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier1ItemDark) };
            bRanking = new RankingVisual { foregroundColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier2Item),
                foregroundSprite = bRankingSprite,
                backgroundColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier2ItemDark) };
            aRanking = new RankingVisual { foregroundColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier3Item),
                foregroundSprite = aRankingSprite,
                backgroundColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier3ItemDark) };
            sRanking = new RankingVisual { foregroundColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.BossItem),
                foregroundSprite = sRankingSprite,
                backgroundColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.BossItemDark)
            };
        }
    }
    public struct RankingVisual
    {
        public Color foregroundColor;
        public AssetReferenceSprite foregroundSprite;
        public Color backgroundColor;
    }
}
