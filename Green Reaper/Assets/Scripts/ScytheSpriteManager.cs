using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScytheSpriteManager : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer handle;
    [SerializeField]
    private SpriteRenderer blade;

    [SerializeField]
    private Sprite[] handleSprites;

    [SerializeField]
    private Sprite[] bladeSprites;

    private int currentHandle;
    private int currentBlade;

    public void SetHandle(int index)
    {
        handle.sprite = handleSprites[index];
        currentHandle = index;
    }

    public void SetBlade(int index)
    {
        blade.sprite = bladeSprites[index];
        currentBlade = index;
    }

    public void SetSprites(int handleIndex, int bladeIndex)
    {
        SetHandle(handleIndex);
        SetBlade(bladeIndex);
    }

    public void SetFromSpriteManager(ScytheSpriteManager other)
    {
        if (handleSprites != other.handleSprites)
            throw new ArgumentException("Handle sprites for this and other Scythe Sprite Manager do not match.");
        else if (bladeSprites != other.bladeSprites)
            throw new ArgumentException("Blade sprites for this and other Scythe Sprite Manager do not match.");

        SetSprites(other.currentHandle, currentBlade);
    }
}
