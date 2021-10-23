using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buffs
{
    /// <summary>
    /// An object that can hold buffs
    /// (e.g. A stat)
    /// </summary>
    /// <typeparam name="T">What kind of information the object holds. (typically
    /// an int or double).
    /// </typeparam>
    public interface BuffHolder<T>
    {
        /// <summary>
        /// Adds a buff of the target type to this object.
        /// </summary>
        /// <param name="b">The buff to add.</param>
        void AddBuff(Buff<T> b);

        /// <summary>
        /// Returns whether this object has any active buffs.
        /// </summary>
        /// <returns>Whether this object has any active buffs.</returns>
        bool HasBuffs();

        /// <summary>
        /// Retrieves the buff on this object with the given name. Returns null if no buffs with the given name are found.
        /// </summary>
        /// <param name="name">The name of the buff to retrieve.</param>
        /// <returns>The buff on this object.</returns>
        Buff<T> GetBuff(string name);

        /// <summary>
        /// Cycles through all buffs and does three things:
        /// 1. Checks that all buffs are active.
        /// 2. Wipes (throws away) inactive buffs.
        /// 3. Updates the information this object holds by passing it into the buffs.
        /// </summary>
        void CheckActiveBuffs();
    }
}
