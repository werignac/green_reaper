using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buffs
{
    /// <summary>
    /// A class that waits for a given amount of time in real-time
    /// (assuming its target stat calls IsActive every frame).
    /// </summary>
    public class BuffWaiter
    {
        /// <summary>
        /// How long the waiter has been running for.
        /// </summary>
        private float age = 0;
        /// <summary>
        /// How long the waiter will run for.
        /// </summary>
        private float lifeTime;

        /// <param name="time">How long the waiter will run for.</param>
        public BuffWaiter(float time)
        {
            lifeTime = time;
        }

        /// <summary>
        /// Updates the age of the waiter and
        /// gets whether the waiter has finished waiting yet.
        /// </summary>
        /// <returns>Whether the waiter has finished waiting.</returns>
        public bool IsActive()
        {
            age += Time.deltaTime;
            return age < lifeTime;
        }
    }
}
