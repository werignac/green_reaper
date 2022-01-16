using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    private bool useMoneySpentOnUpgradesNotNumberOfUpgrades;
    [SerializeField]
    private float amountToStealCoefficient = 1;
    [SerializeField]
    private int minimumAmountToSteal = 10;
    [SerializeField]
    private int maximumAmountToSteal = 100;
    [SerializeField]
    private float timeToEscape;
    [SerializeField]
    private float timeToSteal;

    private int coinsActuallyStolen;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool coinsStolen;
    private static bool roundEnded;
    [SerializeField]
    private SpriteRenderer spRender;

    public UnityEvent<int> onSteal = new UnityEvent<int>();
    public UnityEvent onEscape = new UnityEvent();

    [SerializeField]
    private int bonusMoney = 5;

    [SerializeField]
    private UnityEvent<int> startingMoney;

    // Start is called before the first frame update
    void Start()
    {
        if (roundEnded)
        {
            DieWithoutGivingCoins();
            return;
        }
            
        Initialize();
        player = HarvestState.instance.playerInstance.transform;
        HarvestState.instance.roundEnd.AddListener(RoundEnd);
        rb = this.GetComponent<Rigidbody2D>();
        coinsStolen = false;

        startingMoney?.Invoke(bonusMoney);
    }

    // Update is called once per frame
    void Update()
    {
        // Difference between this object and the player.
        Vector3 direction = player.position - this.transform.position;
        direction.Normalize();
        movement = direction;

        // Flip sprite to movement direction while considering whether or not the coins have been stolen.
        if (!coinsStolen) // Face towards player while stealing.
        {
            if (direction.x > 0)
                spRender.flipX = false;
            else
                spRender.flipX = true;
        }
        else // Face away from player after stealing.
        {
            if (direction.x > 0)
                spRender.flipX = true;
            else
                spRender.flipX = false;
        }
        
    }

    private void FixedUpdate()
    {
        MoveCharacter(movement);

        // If the coins have already been stolen the monster will try to escape in the given amount of time. 
        if (coinsStolen)
        {
            timeToEscape -= Time.deltaTime;
            // Just disable it because onDeath will return the coins.
            if (timeToEscape <= 0)
            {
                onEscape?.Invoke();
                Destroy(gameObject);
            }
        }
        else // If they haven't stolen your stuff, the monster can time out and die without ever stealing the player's stuff.
        {
            timeToSteal -= Time.deltaTime;
            if(timeToSteal <= 0)
            {
                DieWithoutGivingCoins();
            }
        }
    }

    private void DieWithoutGivingCoins()
    {
        deathEvent?.Invoke(GetPlantType());
        Destroy(gameObject);
    }

    /// <summary>
    /// Moves the monster towards or away from the player depending if the coins have already been stolen.
    /// </summary>
    /// <param name="direction"></param>
    private void MoveCharacter(Vector2 direction)
    {
        if (coinsStolen)
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
        int stealMetric;

        if (useMoneySpentOnUpgradesNotNumberOfUpgrades)
            stealMetric = GameManager.instance.GetMoneySpentOnUpgrades();
        else
            stealMetric = GameManager.instance.GetNumberOfUpgradesPurchased();

        int coinsToSteal = Mathf.Min(Mathf.Max(minimumAmountToSteal ,(int)(stealMetric * amountToStealCoefficient)), maximumAmountToSteal);
        coinsActuallyStolen = HarvestState.instance.DecrementScore(coinsToSteal);
        onSteal?.Invoke(coinsActuallyStolen + bonusMoney);
    }

    public bool HasStolenCoins()
    {
        return coinsStolen;
    }

    protected override void OnDeath()
    {
        HarvestState.instance.IncrementScore(coinsActuallyStolen + bonusMoney);
        deathEvent?.Invoke(GetPlantType());
        QuestManager.instance.PlantDied(GetPlantType());
        Destroy(gameObject);
    }

    public override void ChangeHealth(int amountChanged)
    {
        health?.SetValue(Mathf.Clamp(health.GetValue() + amountChanged, 0, baseHealth));
    }

    // When the round ends delete this object.
    private void RoundEnd(int a)
    {
        roundEnded = true;
        onEscape.Invoke();
        Destroy(gameObject);
    }

    public static void EnableRootMonsters()
    {
        roundEnded = false;
    }
}
