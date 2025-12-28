using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using R2API;

namespace StageRanking
{
    public class StageRankingPanel : MonoBehaviour
    {
        public static void CreatePanel(List<Score> scores)
        {
            CreateDebugChatMessage(scores);
        }

        public static void CreateDebugChatMessage(List<Score> scores)
        {
            Chat.AddMessage("==== Stage Ranking Panel ====");
            int totalScore = 0;
            foreach (var item in scores)
            {
                Chat.AddMessage(item.ToString());
                totalScore += item.score;
            }
            Chat.AddMessage($"<size=110%>{Util.PointsTextFormat(totalScore)} ==== </size><size=130%>{Util.GetRanking(totalScore)}</size>");
        }
    }
}
