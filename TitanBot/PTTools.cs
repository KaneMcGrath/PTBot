using ICSharpCode.SharpZipLib.Tar;
using UnityEngine;

namespace TitanBot
{
    public static class PTTools
    {
        public static bool debugPlayerData = true;

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
        /// <summary>
        /// Advanced position estimation
        /// raycasted collision detection and maybe hook spiral prediction
        /// but that will be later on
        /// </summary>
        /// <param name="player"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 LonelySteelSheetFlyer(GameObject player, float t)
        {
            Vector3 prediction = predictPlayerMotion(player, t);
            //if (Physics.Raycast(player.transform.position, prediction,))
            return Vector3.zero;
        }
    }
}