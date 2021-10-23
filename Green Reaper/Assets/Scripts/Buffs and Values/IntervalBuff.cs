using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Buffs
{
    public class IntervalBuff<T> : Buff<T>
    {
        private string name;
        private BuffType buffType;
        private float duration;
        private float interval;
        private float progress;
        private float totalProgress;
        private Func<T, T> intervalEffect;
        private Func<T, T> notEffect;
        private Action wipe;

        public string Name => name;

        public IntervalBuff(float _duration, float _interval, Func<T, T> _intervalEffect, BuffType bt, string _name) : this(_duration, _interval, _intervalEffect, null, null, bt, _name) { }

        public IntervalBuff(float _duration, float _interval, Func<T, T> _intervalEffect, Func<T, T> _notEffect, BuffType bt, string _name) : this(_duration, _interval, _intervalEffect, _notEffect, null, bt, _name) { }

        public IntervalBuff(float _duration, float _interval, Func<T, T> _intervalEffect, Func<T,T> _notEffect, Action _wipe, BuffType bt, string _name)
        {
            duration = _duration;
            interval = _interval;

            intervalEffect = _intervalEffect;
            notEffect = _notEffect;

            progress = 0;
            totalProgress = 0;

            wipe = _wipe;
            buffType = bt;
            name = _name;
        }

        public T Affect(T value)
        {
            progress += Time.deltaTime;

            if (progress >= interval)
            {
                totalProgress += interval;
                progress = 0;

                return intervalEffect(value);
            }
            else if (notEffect != null)
            {
                return notEffect(value);
            }

            return value;
        }

        public void Combine(Buff<T> other)
        {
            if (! name.Equals(other.Name))
                throw new ArgumentException("Buffs must be of the same type to combine. Received buff of " + other.Name + " while is buff of type " + Name + ".");

            IntervalBuff<T> castedOther = (IntervalBuff<T>)other;
            totalProgress -= Math.Min(totalProgress, castedOther.duration - castedOther.totalProgress);
        }

        public BuffType GetBuffType()
        {
            return buffType;
        }

        public bool IsActive()
        {
            return totalProgress < duration;
        }

        public void Wipe()
        {
            wipe?.Invoke();
        }
    }
}
