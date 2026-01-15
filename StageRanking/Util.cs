using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
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
            return Language.GetStringFormatted("STAT_NAME_VALUE_FORMAT", Language.GetString(scoreName), score);
        }
        public static string PointsTextFormat(int points)
        {
            return Language.GetStringFormatted("STAT_POINTS_FORMAT", points);
        }
        public static Ranking GetRanking(int score, int scoreRequirement)
        {
            if (score >= scoreRequirement * Config.SRankRequirement().Value) return Ranking.S;
            if (score >= scoreRequirement * Config.ARankRequirement().Value) return Ranking.A;
            if (score >= scoreRequirement * Config.BRankRequirement().Value) return Ranking.B;
            if (score >= scoreRequirement * Config.CRankRequirement().Value) return Ranking.C;
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
        public static bool GetIsAnimationSpeedLong()
        {
            if (NetworkServer.active)
            {
                return Config.AnimationDuration().Value != Config.AnimationSpeed.Short;
            }
            return Config.AnimationDuration().Value == Config.AnimationSpeed.Long;
            // Should check if host has the mod too. If they do and the animation speed is long, then client default can be long too. I don't know how to check that
        }
    }
}
