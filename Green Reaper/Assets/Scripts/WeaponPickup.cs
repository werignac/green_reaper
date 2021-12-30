using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A pickup that re-enables the player to attack after it has been disabled.
/// The pickup's sprite is given through the initialize function, otherwise it will not have one.
/// </summary>
public class WeaponPickup : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private bool pickupEanbled;
    
    /// <summary>
    /// Sets the sprite of the pickup to be the same as the player's weapon.
    /// Sets the activity of this object to be false.
    /// </summary>
    /// <param name="weaponSprite">Sprite of the player's weapon.</param>
    public void Initialize(Sprite weaponSprite)
    {
        spriteRenderer.sprite = weaponSprite;
        SetActivity(false);
        pickupEanbled = false;
    }

    /// <summary>
    /// Enables or disables the game object based on the given boolean value.
    /// </summary>
    /// <param name="active">Enabled if true, Disabled if false.</param>
    public void SetActivity(bool active)
    {
        gameObject.SetActive(active);
    }

    /// <summary>
    /// Disables the player from attacking.
    /// </summary>
    public void DisablePlayerAttack()
    {
        HarvestState.instance.playerInstance.GetComponent<PlayerController>().TurnOffPlayerAttack();
    }

    /// <summary>
    /// Parents the pickup to the given game object.
    /// If the parent is null the game object becomes unparented.
    /// </summary>
    /// <param name="parent">Transform to parent to.</param>
    public void SetParent(Transform parent)
    {
        this.gameObject.transform.parent = parent.transform;
    }

    /// <summary>
    /// Sets the global position of the pickup.
    /// </summary>
    /// <param name="position">Desired location.</param>
    public void SetGlobalPosition(Vector2 position)
    {
        this.gameObject.transform.position = position;
    }

    /// <summary>
    /// Sets the local position of the pickup.
    /// </summary>
    /// <param name="position">Desired location.</param>
    public void SetLocalPosition(Vector2 position)
    {
        this.gameObject.transform.localPosition = position;
    }

    /// <summary>
    /// Must be called to enable the pickup.
    /// </summary>
    public void EnablePickup()
    {
        pickupEanbled = true;
    }

    /// <summary>
    /// If the player collides with the pickup, then re-enable the ability to attack.
    /// </summary>
    /// <param name="collision">The only collision that results in change is with the player.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && pickupEanbled)
        {
            HarvestState.instance.playerInstance.GetComponent<PlayerController>().TurnOnPlayerAttack();
            SetActivity(false);
            pickupEanbled = false;
        }
    }
}
