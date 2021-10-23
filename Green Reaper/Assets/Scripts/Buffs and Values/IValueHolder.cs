using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public abstract class IValueHolder<T> where T : IComparable
{
    public UnityEvent<T> valueChanged = new UnityEvent<T>();
    public UnityEvent<T> valueIncreased = new UnityEvent<T>();
    public UnityEvent<T> valueDecreased = new UnityEvent<T>();

    public abstract T GetValue();

    public void CheckForChange(T last, T current)
    {
        if (!last.Equals(current))
        {
            valueChanged.Invoke(current);

            if (current.CompareTo(last) > 0)
                valueIncreased.Invoke(current);
            else
                valueDecreased.Invoke(current);
        }
    }
}
