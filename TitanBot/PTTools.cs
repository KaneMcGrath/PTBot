using UnityEngine;

namespace TitanBot
{
    internal static class PTTools
    {

        /// <summary>
        /// Estimates a players position after a given amount of time
        /// dosent take acceleration, hooks, or collisions into account
        /// </summary>
        /// <param name="player"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 predictPlayerMotion(GameObject player, float t)
        {
            return player.transform.position + player.transform.rigidbody.velocity * t + new Vector3(0f, -20f * player.rigidbody.mass, 0f) * t * t * 0.5f;
        }
    }
}