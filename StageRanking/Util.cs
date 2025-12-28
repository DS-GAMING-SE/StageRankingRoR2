using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace StageRanking
{
    public static class Util
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampedNonlinearLerp(int min, int max, float t, float strength)
        {
            if (strength <= 0) { return (int)Mathf.Lerp(min, max, t); }
            return (int)Math.Clamp((1 + 1 / strength) * RoR2.Util.Hyperbolic(t * strength) * (max - min) + min, min, max);
        }
        public static string ScoreTextFormat(string scoreName, int score)
        {
            return $"{scoreName}: {PointsTextFormat(score)}";
        }
        public static string PointsTextFormat(int points)
        {
            return $"<style=cUserSetting>{points}</style> pts.";
        }
        public static Ranking GetRanking(int score)
        {
            if (score >= Config.SRankRequirement().Value) return Ranking.S;
            if (score >= Config.ARankRequirement().Value) return Ranking.A;
            if (score >= Config.BRankRequirement().Value) return Ranking.B;
            if (score >= Config.CRankRequirement().Value) return Ranking.C;
            return Ranking.D;
        }
        public enum Ranking
        {
            D,
            C,
            B,
            A,
            S
        }
    }
}
