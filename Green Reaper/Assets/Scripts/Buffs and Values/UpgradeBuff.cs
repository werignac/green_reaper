using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Buffs
{
    public class UpgradeBuff<T> : FuncBuff<T>
    {
        private Action<UpgradeBuff<T>, UpgradeBuff<T>> combiner;

        public UpgradeBuff(Func<T, T> _affecter, Action<UpgradeBuff<T>,UpgradeBuff<T>> _combiner, BuffType type, Action _wipe, string _name) : base(_affecter, () => true, type, _wipe, _name)
        {
            combiner = _combiner;
        }

        public override void Combine(Buff<T> other)
        {
            if (other.Name != Name)
                throw new ArgumentException("Buffs must be of the same type to combine. Received buff of " + other.Name + " while is buff of type " + Name + ".");

            combiner(this, (UpgradeBuff<T>) other);
        }
    }
}
