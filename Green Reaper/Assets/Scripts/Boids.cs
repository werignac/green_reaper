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


        public IndividualBoid(Vector2 initialPositionRange, Vector2 initialSpeedRange, GameObject prefab, GameObject parentObject)
        {
            parent = parentObject;

            // Object is defaulted to be off.
            obj = Instantiate(prefab);
            ParentBoid();
            obj.SetActive(false);
            spriteRenderer = obj.GetComponent<SpriteRenderer>();

            GenerateRandomStartPosition(initialPositionRange);

            // Range between 0 and 1 when cast to an int.
            int yesOrNo = UnityEngine.Random.Range(0, 2);

            if (yesOrNo == 0)
                turnsRight = false;
            else
                turnsRight = true;

            // Velocities for both the X and Y directions are randomized.
            velocity = new Vector2();
            velocity.x = UnityEngine.Random.Range(initialSpeedRange.x, initialSpeedRange.y);
            velocity.y = UnityEngine.Random.Range(initialSpeedRange.x, initialSpeedRange.y);
        }

        /// <summary>
        /// Random starting position.
        /// </summary>
        /// <param name="initialPositionRange">Range to create a starting position.</param>
        public void GenerateRandomStartPosition(Vector2 initialPositionRange)
        {
            float x = UnityEngine.Random.Range(initialPositionRange.x * -1, initialPositionRange.x);
            float y = UnityEngine.Random.Range(initialPositionRange.y * -1, initialPositionRange.y);
            SetPosition(new Vector2(x, y));
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
    public int numBoids;
    public int visualRange;
    public float centeringFactor;
    public float minDistance;
    public float avoidFactor;
    public float matchingFactor;
    public float speedlimit;
    public float minimumSpeedLimit;
    public float escapeSpeed;
    public float roamRadius;
    public float turnSpeedDegrees;
    public float turnSpeedBoost;
    public float centerOnPlayerBias;
    [SerializeField, Range(0, 1)]
    public float outOfRangeAngle;
    public float debuffTime;
    public float closeEnoughToSteal;
    // Instance of the boids to allow other objects to interact with it.
    public static Boids instance;

    private bool simulating = false;

    /// <summary>
    /// GET RID OF THIS WHEN IMPLEMENTING WITH PLAYER.
    /// </summary>
    //public GameObject playerWeapon;

    private GameObject playerWeaponInstance;
    private float debuffTimeRemaining;
    private bool leadBoidChosen = false;
    private bool scattering = false;
    private GameObject player;
    private bool initialized = false;
    private Vector3 positionForWeapon;


    /// <summary>
    /// The x range will be defined as, xRange = [-x, x], and similarly for Y.
    /// Creates a box with the two boundaries that the boid can spawn in.
    /// </summary>
    public Vector2 startingPositionRange;

    private List<IndividualBoid> boids;

    private void Initialize()
    {
        // Boids should only be initialized once.
        if (initialized)
        {
            return;
        }

        initialized = true;

        // Copy the current weapon of the player. 
        GameObject playerWeapon = GameManager.instance.upgrades.GetWeapon().gameObject;
        playerWeapon.SetActive(false);
        playerWeaponInstance = Instantiate(playerWeapon);
        playerWeaponInstance.SetActive(false);

        player = HarvestState.instance.playerInstance;

        boids = new List<IndividualBoid>();
        for (int i = 0; i < numBoids; ++i)
            boids.Add(new IndividualBoid(startingPositionRange, new Vector2(minimumSpeedLimit, speedlimit), bird, player));
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
        simulating = true;
        leadBoidChosen = false;
        scattering = false;
        foreach (IndividualBoid boid in boids)
        {
            boid.ParentBoid();
            boid.obj.SetActive(true);
            boid.GenerateRandomStartPosition(startingPositionRange);
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
                if (!scattering)
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

            Vector2 newBoidDir = boid.GetVelocity();
            newBoidDir.x = newBoidDir.x * Mathf.Cos(Mathf.Deg2Rad * turnDirectionDegrees) - newBoidDir.y * Mathf.Sin(Mathf.Deg2Rad * turnDirectionDegrees);
            newBoidDir.y = newBoidDir.x * Mathf.Sin(Mathf.Deg2Rad * turnDirectionDegrees) + newBoidDir.y * Mathf.Cos(Mathf.Deg2Rad * turnDirectionDegrees);

            // Prevent them from not moving.
            if (newBoidDir.x == 0 && newBoidDir.y == 0)
            {
                float x = UnityEngine.Random.Range(startingPositionRange.x * -1, startingPositionRange.x);
                float y = UnityEngine.Random.Range(startingPositionRange.y * -1, startingPositionRange.y);
                newBoidDir = new Vector2(x, y);
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
            playerWeaponInstance.transform.parent = null;
            playerWeaponInstance.transform.position = positionForWeapon;
            StopSimulation();
        }
    }

    private void StealAndScatter(IndividualBoid leadBoid)
    {
        leadBoid.UnparentBoid();
        scattering = true;

        // Parent the weapon to the lead boid.
        playerWeaponInstance.transform.parent = leadBoid.obj.transform;
        playerWeaponInstance.transform.localPosition = Vector2.zero;
        playerWeaponInstance.SetActive(true);

        // Point the lead boid towards a random tile and set the speed to the escape speed.
        positionForWeapon = CornCoordinatorByWeight.instance.RandomTileToWorldCoordinates();
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