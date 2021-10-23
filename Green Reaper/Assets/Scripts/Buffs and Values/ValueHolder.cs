using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ValueHolder<T> : IValueHolder<T> where T : IComparable
{
    private T value;

    public ValueHolder(T startVal)
    {
        value = startVal;
    }

    public override T GetValue()
    {
        return value;
    }

    public void SetValue(T newValue)
    {
        T last = value;
        value = newValue;
        CheckForChange(last, value);
    }
}
