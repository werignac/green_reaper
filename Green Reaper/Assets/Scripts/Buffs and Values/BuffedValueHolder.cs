using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;
using UnityEngine.Events;
using System;

public class BuffedValueHolder<T> : IValueHolder<T>, BuffHolder<T> where T : IComparable
{
    private readonly T baseValue;

    private T value;

    private Dictionary<string, Buff<T>> buffs;

    private HashSet<string> blackListByName;

    private HashSet<BuffType> blackListByType;

    public override T GetValue()
    {
        return value;
    }

    public BuffedValueHolder(T _baseValue)
    {
        baseValue = _baseValue;
        buffs = new Dictionary<string, Buff<T>>();
        value = _baseValue;
        blackListByName = new HashSet<string>();
        blackListByType = new HashSet<BuffType>();
    }

    /// <summary>
    /// Adds a buff to the attribute.
    /// Combines attributes that have the same name.
    /// </summary>
    /// <param name="b">The buff to add</param>
    public bool AddBuff(Buff<T> b)
    {
        if (!blackListByName.Contains(b.Name) && !blackListByType.Contains(b.GetBuffType()))
        {
            if (!buffs.ContainsKey(b.Name))
                buffs.Add(b.Name, b);
            else
                buffs[b.Name].Combine(b);

            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns whether this attribute has any active buffs.
    /// </summary>
    /// <returns>Whether this attribute has any active buffs.</returns>
    public bool HasBuffs()
    {
        return buffs.Count > 0;
    }

    /// <summary>
    /// Checks which buffs are still active and updates
    /// the value of this attribute.
    /// </summary>
    public void CheckActiveBuffs()
    {
        T last = value;

        //The value is a modified version of the base value.
        value = baseValue;
        //Collect all the buffs to remove.
        HashSet<string> toRemove = new HashSet<string>();

        foreach (Buff<T> b in buffs.Values)
        {
            if (b.IsActive())
            {//Have the buff affect the value of this attribute.
                value = b.Affect(value);
            }
            else
            {//Wipe the buff if it is no longer active.
                b.Wipe();
                toRemove.Add(b.Name);
            }
        }
        //Remove all inactive buffs from this attribute.
        foreach (string remove in toRemove)
            buffs.Remove(remove);

        CheckForChange(last, value);
    }

    public Buff<T> GetBuff(string name)
    {
        if (buffs.TryGetValue(name, out Buff<T> buff))
            return buff;
        return null;
    }

    public void BlackList(string name)
    {
        Remove(name);
        blackListByName.Add(name);
    }

    public void UnblackList(string name)
    {
        blackListByName.Remove(name);
    }

    public void BlackList(BuffType type)
    {
        HashSet<Buff<T>> toRemove = new HashSet<Buff<T>>();

        foreach(Buff<T> buff in buffs.Values)
            if (buff.GetBuffType() == type)
                toRemove.Add(buff);

        foreach (Buff<T> buff in toRemove)
        {
            buff.Wipe();
            buffs.Remove(buff.Name);
        }

        blackListByType.Add(type);
    }

    public void UnblackList(BuffType type)
    {
        blackListByType.Remove(type);
    }

    public bool Remove(string name)
    {
        if (buffs.TryGetValue(name, out Buff<T> buff))
        {
            buff.Wipe();
            buffs.Remove(name);
            return true;
        }

        return false;
    }
}
