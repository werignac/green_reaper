using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for the monster that chases the player and steals their coins. 
/// </summary>
public class RootMonster : PlantHealth
{
    private Transform player;
    [SerializeField]
    private float chaseMovementSpeed;
    [SerializeField]
    private float escapeMovementSpeed;
    [SerializeField]
    private int coinsToSteal;
    [SerializeField]
    private float timeToEscape;

    private int coinsActuallyStolen;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool coinsStolen;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        player = HarvestState.instance.playerInstance.transform;
        HarvestState.instance.roundEnd.AddListener(RoundEnd);
       rb = this.GetComponent<Rigidbody2D>();
        coinsStolen = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Difference between this object and the player.
        Vector3 direction = player.position - this.transform.position;
        // Rotate this object to face player.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        rb.rotation = angle;

        direction.Normalize();
        movement = direction;
    }

    private void FixedUpdate()
    {
        MoveCharacter(movement);
        
        // If the coins have already been stolen the monster will try to escape in the given amount of time. 
        if(coinsStolen)
        {
            timeToEscape -= Time.deltaTime;
            // Just disable it because onDeath will return the coins.
            if(timeToEscape <= 0)
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// Moves the monster towards or away from the player depending if the coins have already been stolen.
    /// </summary>
    /// <param name="direction"></param>
    private void MoveCharacter(Vector2 direction)
    {
        if(coinsStolen)
        {
            rb.MovePosition((Vector2)transform.position + (-1 * direction * escapeMovementSpeed * Time.deltaTime));
        }
        else
        {
            rb.MovePosition((Vector2)transform.position + (direction * chaseMovementSpeed * Time.deltaTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && !coinsStolen)
        {
            StealCoins();
            coinsStolen = true;
        }
            
    }

    private void StealCoins()
    {
        coinsActuallyStolen = HarvestState.instance.DecrementScore(coinsToSteal);
    }

    protected override void OnDeath()
    {
        HarvestState.instance.IncrementScore(coinsActuallyStolen);
        Destroy(gameObject);
    }

    public override void ChangeHealth(int amountChanged)
    {
        if(coinsStolen)
            health.SetValue(Mathf.Clamp(health.GetValue() + amountChanged, 0, baseHealth));
    }

    // When the round ends delete this object.
    private void RoundEnd(int a)
    {
        Destroy(gameObject);
    }
}