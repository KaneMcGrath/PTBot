using UnityEngine;

namespace TitanBot
{
    public static class PTTools
    {
        public static bool debugPlayerData = false;
        public static Difficulty difficulty = Difficulty.VeryVeryHard;
        private static float RandomTimer;
        private static float xRand = 0f;
        private static float yRand = 0f;
        private static float zRand = 0f;
        private static float tRand = 0f;
        /// <summary>
        /// Estimates a players position after a given amount of time
        /// dosent take acceleration, hooks, or collisions into account
        /// randomness is applied based on the difficulty
        /// </summary>
        /// <param name="player"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 PredictPlayerMotion(GameObject player, float t)
        {
            if (difficulty == Difficulty.VeryVeryHard)
            {
                Vector3 p = player.transform.position + player.transform.rigidbody.velocity * t + new Vector3(0f, -20f * player.rigidbody.mass, 0f) * t * t * 0.5f;
                if (debugPlayerData) CGTools.pointsToTrack.Add(p);
                return p;
            }

            if (CGTools.timer(ref RandomTimer, 0.1f))
            {
                int d = (int)difficulty;
                xRand = d + d * UnityEngine.Random.Range(-1f, 1f);
                yRand = d + d * UnityEngine.Random.Range(-1f, 1f);
                zRand = d + d * UnityEngine.Random.Range(-1f, 1f);
                tRand = d * UnityEngine.Random.Range(0f, 0.2f);
            }
            float time = t + tRand;
            Vector3 difficultyAdjusted = new Vector3(xRand, yRand, zRand);
            Vector3 point = (player.transform.position + (difficultyAdjusted * 5f)) + (player.transform.rigidbody.velocity + difficultyAdjusted) * time + new Vector3(0f, -20f * player.rigidbody.mass, 0f) * time * time * 0.5f;
            
            if (debugPlayerData) CGTools.pointsToTrack.Add(point);
            return point;
        }

    }

    /// <summary>
    /// affects the accuracy of player motion prediction by adding randomness to the prediction
    /// with VeryVeryHard having no randomness
    /// </summary>
    public enum Difficulty
    {
        VeryVeryHard,
        VeryHard,
        Hard,
        Medium,
        Easy,
        VeryEasy
    }
}