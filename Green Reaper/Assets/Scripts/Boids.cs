using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Based on Ben Eater's code for boids.
/// Link: https://github.com/beneater/boids/blob/master/boids.js
/// Implements Boids algorithm.
/// </summary>
public class Boids : MonoBehaviour
{

    /// <summary>
    /// Represents an individual boid.
    /// </summary>
    private class IndividualBoid
    {
        private Vector2 velocity;
        public GameObject obj;
        public bool turnsRight;
        public SpriteRenderer spriteRenderer;
        public bool leadBoid = false;
        private GameObject parent;


        public IndividualBoid(float initailPositionRadius, float initialSpeed, GameObject prefab, GameObject parentObject)
        {
            parent = parentObject;

            // Object is defaulted to be off.
            obj = Instantiate(prefab);
            ParentBoid();
            obj.SetActive(false);
            spriteRenderer = obj.GetComponent<SpriteRenderer>();

            InitialzeBoid(initailPositionRadius, initialSpeed);
        }

        public void InitialzeBoid(float initailPositionRadius, float initialSpeed)
        {
            GenerateStartPosition(initailPositionRadius);
            GenerateStartVelocity(initialSpeed);

            // yesOrNo is 0 or 1 when cast to an int.
            int yesOrNo = UnityEngine.Random.Range(0, 2);

            // Determine if the boid turns left or right.
            if (yesOrNo == 0)
                turnsRight = false;
            else
                turnsRight = true;
        }

        /// <summary>
        /// Point the boid at the player, and travel at starting speed.
        /// </summary>
        /// <param name="initialSpeed">Speed the boid should start at.</param>
        private void GenerateStartVelocity(float initialSpeed)
        {
            velocity = obj.transform.localPosition * -1;
            velocity = velocity.normalized * initialSpeed;
        }

        /// <summary>
        /// Random starting position.
        /// </summary>
        /// <param name="initialPositionRange">Range to create a starting position.</param>
        private void GenerateStartPosition(float initailPositionRadius)
        {
            Vector2 randomPosition = UnityEngine.Random.insideUnitCircle;
            
            // A default case to avoid the bird spawning on top of the player.
            if(randomPosition == Vector2.zero)
            {
                randomPosition = Vector2.left;
            }
            
            // Normalize, and then move down that direction to the radius of where the boid should start.
            randomPosition = randomPosition.normalized;
            randomPosition *= initailPositionRadius;

            SetPosition(randomPosition);
        }

        /// <summary>
        /// Changes the local position of the boid to the given x, y values.
        /// Z is set to 0.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        public void SetPosition(Vector2 newPosition)
        {
            obj.transform.localPosition = newPosition;
        }

        public void SetVelocity(Vector2 newVelocity)
        {
            velocity = newVelocity;
        }

        /// <returns>Local position of the boid.</returns>
        public Vector2 GetLocalPosition()
        {
            return (Vector2)obj.transform.localPosition;
        }

        /// <returns>World position of the boid.</returns>
        public Vector2 GetWorldPosition()
        {
            return obj.transform.position;
        }

        /// <returns>Velocity of the boid.</returns>
        public Vector2 GetVelocity()
        {
            return velocity;
        }

        /// <summary>
        /// Changes the world position of the boid to the given x, y values.
        /// Z is set to 0.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        public void SetPositionWorld(Vector2 newPosition)
        {
            obj.transform.position = newPosition;
        }

        public void UnparentBoid()
        {
            obj.transform.parent = null;
        }

        public void ParentBoid()
        {
            obj.transform.parent = parent.transform;
        }
    }


    public GameObject bird;
    public GameObject WeaponPickupPrefab;
    [SerializeField]
    private GameObject hingedScythePrefab;
    private GameObject currentHingedScythe;
    public int numBoids;
    public int visualRange;
    public float centeringFactor;
    public float minDistance;
    public float avoidFactor;
    public float matchingFactor;
    public float speedlimit;
    public float minimumSpeedLimit;
    public float escapeSpeed;
    public float startPositionRadius;
    public float roamRadius;
    public float timeToFlyToPlayer;
    public float turnSpeedDegrees;
    public float turnSpeedBoost;
    public float centerOnPlayerBias;
    [SerializeField, Range(0, 1)]
    public float outOfRangeAngle;
    public float debuffTime;
    public float closeEnoughToSteal;
    public int tileDistanceCrowsDropScythe;
    // Instance of the boids to allow other objects to interact with it.
    public static Boids instance;
    
    

    private float birdFlightGracePeriod;
    private float debuffTimeRemaining;
    private bool leadBoidChosen = false;
    private bool scattering = false;
    private bool simulating = false;
    private bool initialized = false;
    private GameObject player;
    private Vector3 positionForWeapon;
    private List<IndividualBoid> boids;
    private WeaponPickup playerWeaponPickup;

    private void Initialize()
    {
        // Boids should only be initialized once.
        if (initialized)
        {
            return;
        }

        initialized = true;

        // Initialzize the weapon pickup with the sprite of the player's weapon.
        //GameObject playerWeapon = GameManager.instance.upgrades.GetWeapon().gameObject;
        //playerWeaponPickup.Initialize(playerWeapon.GetComponent<SpriteRenderer>().sprite);

        // Instantiate the weapon pickup object and store a reference to it's controller.

        //WeaponPickupPrefab = Instantiate(WeaponPickupPrefab);
        //playerWeaponPickup = WeaponPickupPrefab.GetComponent<WeaponPickup>();

        // Initialize the pickup so that it is ready to be placed.
        //playerWeaponPickup.Initialize();

        player = HarvestState.instance.playerInstance;

        boids = new List<IndividualBoid>();
        for (int i = 0; i < numBoids; ++i)
            boids.Add(new IndividualBoid(startPositionRadius, speedlimit, bird, player));
    }

    private void Awake()
    {
        instance = this;
    }

    public void StartSimulation()
    {
        // If the simulation is currently running, we do not want to restart it.
        if (simulating)
            return; 

        Initialize();
        debuffTimeRemaining = debuffTime;
        birdFlightGracePeriod = timeToFlyToPlayer;
        simulating = true;
        leadBoidChosen = false;
        scattering = false;
        foreach (IndividualBoid boid in boids)
        {
            boid.ParentBoid();
            boid.obj.SetActive(true);
            boid.InitialzeBoid(startPositionRadius, speedlimit);
        }
    }

    public void StopSimulation()
    {
        simulating = false;
        scattering = false;
        foreach (IndividualBoid boid in boids)
        {
            boid.obj.SetActive(false);
            boid.leadBoid = false;
        }
    }

    // Main simulation loop.
    private void FixedUpdate()
    {
        if (simulating)
        {
            debuffTimeRemaining -= Time.deltaTime;
            birdFlightGracePeriod -= Time.deltaTime;

            if (debuffTimeRemaining <= 0)
            {
                if (!leadBoidChosen)
                    ChooseLeadBoid();
            }

            foreach (IndividualBoid boid in boids)
            {
                // A lead boid is taken control of to initiate scattering.
                if (boid.leadBoid && !scattering)
                {
                    // Point the lead boid directly at the player.
                    boid.SetVelocity(-1 * boid.GetLocalPosition());
                    clampSpeed(boid);
                    MoveBoid(boid);
                    continue;
                }

                // Update all boids according to each rule when not scattering.
                // Bird flight grace period allows the birds to fly towards the player at the start without to center them more on the player.
                if (!scattering && birdFlightGracePeriod <= 0)
                {
                    FlyTowardsCenter(boid);
                    AvoidOthers(boid);
                    MatchVelocity(boid);
                    KeepWithinBounds(boid);
                    clampSpeed(boid);
                }

                MoveBoid(boid);

                // If the lead boid is close enough to drop the player's weapon.
                if (boid.leadBoid)
                    CheckForDropOffAndEnd(boid);
            }

        }
    }



    private void ChooseLeadBoid()
    {
        int chosenBoid = UnityEngine.Random.Range(0, numBoids);
        boids[chosenBoid].leadBoid = true;
        leadBoidChosen = true;
    }

    //Find the center of mass of the other boids and adjust velocity slightly to
    //point towards the center of mass.
    private void FlyTowardsCenter(IndividualBoid boid)
    {
        float centerX = 0;
        float centerY = 0;
        int numNeighbors = 0;

        foreach (IndividualBoid otherBoid in boids)
        {
            float distBetween = Vector2.Distance(boid.GetLocalPosition(), otherBoid.GetLocalPosition());

            if (distBetween < visualRange)
            {
                centerX += otherBoid.GetLocalPosition().x;
                centerY += otherBoid.GetLocalPosition().y;
                numNeighbors += 1;
            }
        }

        if (numNeighbors > 0)
        {
            centerX /= numNeighbors;
            centerY /= numNeighbors;
            Vector2 centerPosition = new Vector2(centerX, centerY);

            // How much the velocity of the boid should change.
            Vector2 centeringVelocity = (centerPosition - boid.GetLocalPosition()) * centeringFactor;
            // Change boid's velocity.
            boid.SetVelocity(boid.GetVelocity() + centeringVelocity);
        }
    }

    // Constrain a boid to within the window. If it gets too close to an edge,
    // nudge it back in and reverse its direction.
    private void KeepWithinBounds(IndividualBoid boid)
    {
        // Distance from this.
        if (boid.GetLocalPosition().magnitude > roamRadius)
        {
            // Depnding on whether or not the boid turns right this will either be positive or negative turnSpeedDegrees.
            float turnDirectionDegrees = turnSpeedDegrees;
            if (boid.turnsRight)
                turnDirectionDegrees *= -1;

            // Turn the boid.
            Vector2 newBoidDir = boid.GetVelocity();
            newBoidDir.x = newBoidDir.x * Mathf.Cos(Mathf.Deg2Rad * turnDirectionDegrees) - newBoidDir.y * Mathf.Sin(Mathf.Deg2Rad * turnDirectionDegrees);
            newBoidDir.y = newBoidDir.x * Mathf.Sin(Mathf.Deg2Rad * turnDirectionDegrees) + newBoidDir.y * Mathf.Cos(Mathf.Deg2Rad * turnDirectionDegrees);

            // Prevent them from not moving. If they stop moving point them at the player.
            if (newBoidDir.x == 0 && newBoidDir.y == 0)
            {
                newBoidDir = boid.GetLocalPosition() * -1;
                newBoidDir = newBoidDir.normalized * minimumSpeedLimit;
            }

            // Apply boost.
            newBoidDir *= turnSpeedBoost;

            boid.SetVelocity(newBoidDir);

            // drag boid to the player/origin.
            if (Vector2.Dot(boid.GetVelocity(), boid.GetLocalPosition()) > 0)
            {
                Vector2 antiVelocity = boid.GetVelocity();
                antiVelocity -= boid.GetLocalPosition() * centerOnPlayerBias;
                boid.SetVelocity(antiVelocity);
            }
        }
    }

    // Move away from other boids that are too close to avoid colliding
    private void AvoidOthers(IndividualBoid boid)
    {

        float moveX = 0;
        float moveY = 0;
        foreach (IndividualBoid otherBoid in boids)
        {
            if (otherBoid != boid)
            {
                float distBetween = Vector2.Distance(boid.GetLocalPosition(), otherBoid.GetLocalPosition());

                if (distBetween < minDistance)
                {
                    moveX += boid.GetLocalPosition().x - otherBoid.GetLocalPosition().x;
                    moveY += boid.GetLocalPosition().y - otherBoid.GetLocalPosition().y;
                }
            }
        }

        boid.SetVelocity(boid.GetVelocity() + (new Vector2(moveX, moveY) * avoidFactor));
    }

    // Find the average velocity (speed and direction) of the other boids and
    // adjust velocity slightly to match.
    private void MatchVelocity(IndividualBoid boid)
    {
        Vector2 averageVelocity = new Vector2();
        int numNeighbors = 0;

        foreach (IndividualBoid otherBoid in boids)
        {
            float distBetween = Vector2.Distance(boid.GetLocalPosition(), otherBoid.GetLocalPosition());

            if (distBetween < visualRange)
            {
                averageVelocity += boid.GetVelocity();
                numNeighbors += 1;
            }
        }

        if (numNeighbors > 0)
        {
            // Average the velocity.
            averageVelocity /= numNeighbors;
            Vector2 velocityDifference = averageVelocity - boid.GetVelocity();

            boid.SetVelocity(boid.GetVelocity() + velocityDifference * matchingFactor);
        }
    }

    // speed will naturally vary in flocking behavior, but real animals can't go
    // arbitrarily fast.
    private void clampSpeed(IndividualBoid boid)
    {
        float speed = boid.GetVelocity().magnitude;
        // Prevent the boids from travelling faster than the speed limit. 
        if (speed > speedlimit)
        {
            boid.SetVelocity(boid.GetVelocity() / speed * speedlimit);
        }
        if (speed < minimumSpeedLimit)
        {
            boid.SetVelocity(boid.GetVelocity() / speed * minimumSpeedLimit);
        }
    }

    /// <summary>
    /// Update the position based on the current velocity.
    /// </summary>
    /// <param name="boid">Boid to move.</param>
    private void MoveBoid(IndividualBoid boid)
    {
        // If the lead boid gets close enough to steal scythe.
        if (boid.leadBoid && !scattering)
        {
            if (boid.GetLocalPosition().magnitude < closeEnoughToSteal)
                StealAndScatter(boid);
        }

        if (boid.GetVelocity().x > 0)
            boid.spriteRenderer.flipX = false;
        else
            boid.spriteRenderer.flipX = true;

        // If the boids are scattering, the position needs to be moved relative to the world. 
        if (scattering)
            boid.SetPositionWorld(boid.GetWorldPosition() + boid.GetVelocity());
        // If they are not they need to move relative to the player.
        else
            boid.SetPosition(boid.GetLocalPosition() + boid.GetVelocity());
    }

    /// <summary>
    /// Expects the lead boid to be passed.
    /// Checks if the lead boid is close enough to the dropoff point and then ends the simulation after the weapon is dropped.
    /// </summary>
    /// <param name="boid">The lead boid.</param>
    private void CheckForDropOffAndEnd(IndividualBoid boid)
    {
        float distanceToEndPoint = (boid.GetWorldPosition() - (Vector2)positionForWeapon).magnitude;
        
        if (distanceToEndPoint < closeEnoughToSteal)
        {
            Destroy(currentHingedScythe);

            WeaponPickup weaponPickupInstance = Instantiate(WeaponPickupPrefab).GetComponent<WeaponPickup>();
            weaponPickupInstance.SetActivity(true);
            weaponPickupInstance.SetGlobalPosition(positionForWeapon);
            weaponPickupInstance.EnablePickup();

            StopSimulation();
        }
    }

    private void StealAndScatter(IndividualBoid leadBoid)
    {
        leadBoid.UnparentBoid();
        scattering = true;

        // Parent the weapon pickup to the lead boid.
        WeaponPickup.DisablePlayerAttack();

        currentHingedScythe = Instantiate(hingedScythePrefab, leadBoid.obj.transform.position, Quaternion.identity);
        Rigidbody2D crowRigidbody = leadBoid.obj.GetComponent<Rigidbody2D>();
        crowRigidbody.simulated = true;
        currentHingedScythe.GetComponent<HingeJoint2D>().connectedBody = crowRigidbody;

        // Point the lead boid towards a random tile and set the speed to the escape speed.
        positionForWeapon = CornCoordinatorByWeight.instance.RandomTileDistanceAway(tileDistanceCrowsDropScythe, player.transform.position);
        Vector3 direction = positionForWeapon - leadBoid.obj.transform.position;
        leadBoid.SetVelocity(direction.normalized * escapeSpeed);

        // Scatter every other boid.
        foreach (IndividualBoid boid in boids)
        {
            float speed = boid.GetVelocity().magnitude;
            boid.SetVelocity(boid.GetVelocity() / speed * escapeSpeed);
            boid.UnparentBoid();
        }
    }
}