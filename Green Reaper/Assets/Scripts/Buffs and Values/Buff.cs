using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buffs
{
    /// <summary>
    /// The different kinds of buffs possible.
    /// Natural buffs cannot to removed by external forces.
    /// </summary>
    public enum BuffType { BUFF, DEBUFF, NATURAL}

    /// <summary>
    /// A buff is an effect on a stat over time.
    /// </summary>
    /// <typeparam name="T">The type of object the buff affects.</typeparam>
    public interface Buff<T>
    {
        BuffType GetBuffType();

        /// <summary>
        /// Combines two buffs of the same type 
        /// (typically a name is used to identify two similar Buffs).
        /// </summary>
        /// <param name="other">The other buff to combine.</param>
        void Combine(Buff<T> other);

        /// <summary>
        /// The name or description of the buff.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// How the buff affects its target stat for a period of time.
        /// </summary>
        /// <param name="value">The stat in its current state (may be modified by other buffs beforehand).</param>
        /// <returns>The new version of the stat to be set.</returns>
        T Affect(T value);

        /// <summary>
        /// Whether the buff is still active. If it's not,
        /// it will be removed from the stat.
        /// </summary>
        /// <returns>Whether the buff is active or not.</returns>
        bool IsActive();

        /// <summary>
        /// An action to perform once the buff is removed.
        /// </summary>
        void Wipe();
    }
}
