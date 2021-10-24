using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Buffs
{
    /// <summary>
    /// A buff whose main functions are defined by functors.
    /// </summary>
    public class FuncBuff<T> : Buff<T>
    {
        /// <summary>
        /// The function that defines how the buff affects the target stat.
        /// </summary>
        private Func<T, T> affecter;
        /// <summary>
        /// The function that defines when the buff is active.
        /// </summary>
        private Func<bool> active;
        /// <summary>
        /// The type of the buff.
        /// </summary>
        private BuffType buffType;
        /// <summary>
        /// The function that defines what the buff does when its erased.
        /// Null if there is nothing to do.
        /// </summary>
        private Action wipe;
        /// <summary>
        /// The name of the buff used to identify buff types.
        /// </summary>
        private readonly string name;

        private BuffWaiter timer;


        public FuncBuff(Func<T, T> _affecter, Func<bool> _active, BuffType type, string _name) : this(_affecter, _active, type, null, _name)
        { }

        public FuncBuff(Func<T, T> _affecter, float lifeTime, BuffType type, string _name) : this(_affecter, lifeTime, type, null, _name)
        { }

        public FuncBuff(Func<T, T> _affecter, float lifeTime, BuffType type, Action _wipe, string _name) : this(_affecter, ()=>false, type, _wipe, _name)
        {
            timer = new BuffWaiter(lifeTime);
            active = () => timer.IsActive();
        }

        public FuncBuff(Func<T, T> _affecter, Func<bool> _active, BuffType type, Action _wipe, string _name)
        {
            affecter = _affecter;
            active = _active;
            buffType = type;
            wipe = _wipe;
            name = _name;
        }

        public string Name => name;

        /// <summary>
        /// A wrapper for the affecter function.
        /// </summary>
        public T Affect(T value)
        {
            return affecter(value);
        }

        /// <summary>
        /// FuncBuffs can't combine by default.
        /// </summary>
        public virtual void Combine(Buff<T> other)
        {
            if (other.Name != Name)
                throw new ArgumentException("Buffs must be of the same type to combine. Received buff of " + other.Name + " while is buff of type " + Name + ".");

            if (timer != null)
                timer.ResetAge();
        }

        public BuffType GetBuffType()
        {
            return buffType;
        }

        /// <summary>
        /// Wrapper for the active function.
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return active();
        }

        /// <summary>
        /// Wrapper for the wipe function.
        /// </summary>
        public void Wipe()
        {
            if (!ReferenceEquals(wipe, null))
                wipe();
        }
    }
}
