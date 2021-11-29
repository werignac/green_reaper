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


        public IndividualBoid(Vector2 initialPositionRange, Vector2 initialSpeedRange, GameObject prefab)
        {
            // Object is defaulted to be off.
            obj = Instantiate(prefab);
            obj.SetActive(false);

            // random starting position.
            float x = UnityEngine.Random.Range(initialPositionRange.x * -1, initialPositionRange.x);
            float y = UnityEngine.Random.Range(initialPositionRange.y * -1, initialPositionRange.y);
            SetPosition(new Vector2(x,y));

            // Velocities for both the X and Y directions are randomized.
            velocity = new Vector2();
            velocity.x = UnityEngine.Random.Range(initialSpeedRange.x, initialSpeedRange.y);
            velocity.y = UnityEngine.Random.Range(initialSpeedRange.x, initialSpeedRange.y);
        }

        /// <summary>
        /// Changes the position of the boid to the given x, y values.
        /// Z is set to 0.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        public void SetPosition(Vector2 newPosition)
        {
            obj.transform.position = newPosition;
        }

        public void SetVelocity(Vector2 newVelocity)
        {
            velocity = newVelocity;
        }
        
        /// <returns>Position of the boid.</returns>
        public Vector2 GetPosition ()
        {
            return (Vector2)obj.transform.position;
        }

        /// <returns>Velocity of the boid.</returns>
        public Vector2 GetVelocity()
        {
            return velocity;
        }
    }

    public GameObject bird;
    public int numBoids = 100;
    public int visualRange = 75;
    public float centeringFactor = 0.005f;
    public float minDistance = 20;
    public float avoidFactor = 0.05f;
    public float matchingFactor = 0.05f;
    public float speedlimit = 15;

    /// <summary>
    /// The x range will be defined as, xRange = [-x, x], and similarly for Y.
    /// Creates a box with the two boundaries that the boid can spawn in.
    /// </summary>
    public Vector2 startingPositionRange;

    /// <summary>
    /// Range of speeds will be defined as, speed = [x, y].
    /// </summary>
    public Vector2 startingSpeedRange;

    private List<IndividualBoid> boids;

    // Start is called before the first frame update
    void Start()
    {
        boids = new List<IndividualBoid>();

        for (int i = 0; i < numBoids; ++i)
            boids.Add(new IndividualBoid(startingPositionRange, startingSpeedRange, bird));

        StartSimulation();
    }

    public void StartSimulation()
    {
        foreach (IndividualBoid boid in boids)
        {
            boid.obj.SetActive(true);
        }
    }

    public void StopSimulation()
    {
        foreach (IndividualBoid boid in boids)
        {
            boid.obj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Main simulation loop.
    private void FixedUpdate()
    {
        foreach (IndividualBoid boid in boids)
        {
            // Update the velocities according to each rule
            FlyTowardsCenter(boid);
            AvoidOthers(boid);
            MatchVelocity(boid);
            LimitSpeed(boid);
            //keepWithinBounds(boid);
            MoveBoid(boid);
        }
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
            float distBetween = Vector2.Distance(boid.GetPosition(), otherBoid.GetPosition());

            if (distBetween < visualRange)
            {
                centerX += otherBoid.GetPosition().x;
                centerY += otherBoid.GetPosition().y;
                numNeighbors += 1;
            }
        }

        if (numNeighbors > 0)
        {
            centerX /= numNeighbors;
            centerY /= numNeighbors;
            Vector2 centerPosition = new Vector2(centerX, centerY);

            // How much the velocity of the boid should change.
            Vector2 centeringVelocity = (centerPosition - boid.GetPosition()) * centeringFactor;
            // Change boid's velocity.
            boid.SetVelocity(boid.GetVelocity() + centeringVelocity);
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
                float distBetween = Vector2.Distance(boid.GetPosition(), otherBoid.GetPosition());

                if (distBetween < minDistance)
                {
                    moveX += boid.GetPosition().x - otherBoid.GetPosition().x;
                    moveY += boid.GetPosition().y - otherBoid.GetPosition().y;
                }
            }
        }

        boid.SetVelocity(new Vector2(moveX, moveY) * avoidFactor);
    }

    // Find the average velocity (speed and direction) of the other boids and
    // adjust velocity slightly to match.
    private void MatchVelocity(IndividualBoid boid)
    {
        Vector2 averageVelocity = new Vector2();
        int numNeighbors = 0;

        foreach (IndividualBoid otherBoid in boids)
        {
            float distBetween = Vector2.Distance(boid.GetPosition(), otherBoid.GetPosition());

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

            boid.SetVelocity(velocityDifference * matchingFactor);
        }
    }

    // speed will naturally vary in flocking behavior, but real animals can't go
    // arbitrarily fast.
    private void LimitSpeed(IndividualBoid boid)
    {
        float speed = boid.GetVelocity().magnitude;
        // Prevent the boids from travelling faster than the speed limit. 
        if (speed > speedlimit)
        {
            boid.SetVelocity(boid.GetVelocity() / speed * speedlimit);
        }
    }

    // Update the position based on the current velocity
    private void MoveBoid(IndividualBoid boid)
    {
        Vector2 newPosition = boid.GetPosition() + boid.GetVelocity();
    }
}

//// Size of canvas. These get updated to fill the whole browser.
//let width = 150;
//let height = 150;

//const numBoids = 100;
//const visualRange = 75;

//var boids = [];

//function initBoids()
//{
//    for (var i = 0; i < numBoids; i += 1)
//    {
//        boids[boids.length] = {
//        x: Math.random() * width,
//      y: Math.random() * height,
//      dx: Math.random() * 10 - 5,
//      dy: Math.random() * 10 - 5,
//      history:[],
//    };
//    }
//}

//// TODO: This is naive and inefficient.
//function nClosestBoids(boid, n)
//{
//    // Make a copy
//    const sorted = boids.slice();
//    // Sort the copy by distance from `boid`
//    sorted.sort((a, b) => distance(boid, a) - distance(boid, b));
//    // Return the `n` closest
//    return sorted.slice(1, n + 1);
//}

//// Constrain a boid to within the window. If it gets too close to an edge,
//// nudge it back in and reverse its direction.
//function keepWithinBounds(boid)
//{
//    const margin = 200;
//    const turnFactor = 1;

//    if (boid.x < margin)
//    {
//        boid.dx += turnFactor;
//    }
//    if (boid.x > width - margin)
//    {
//        boid.dx -= turnFactor
//    }
//    if (boid.y < margin)
//    {
//        boid.dy += turnFactor;
//    }
//    if (boid.y > height - margin)
//    {
//        boid.dy -= turnFactor;
//    }
//}

//// Find the center of mass of the other boids and adjust velocity slightly to
//// point towards the center of mass.
//function flyTowardsCenter(boid)
//{
//    const centeringFactor = 0.005; // adjust velocity by this %

//    let centerX = 0;
//    let centerY = 0;
//    let numNeighbors = 0;

//    for (let otherBoid of boids)
//    {
//        if (distance(boid, otherBoid) < visualRange)
//        {
//            centerX += otherBoid.x;
//            centerY += otherBoid.y;
//            numNeighbors += 1;
//        }
//    }

//    if (numNeighbors)
//    {
//        centerX = centerX / numNeighbors;
//        centerY = centerY / numNeighbors;

//        boid.dx += (centerX - boid.x) * centeringFactor;
//        boid.dy += (centerY - boid.y) * centeringFactor;
//    }
//}

//// Move away from other boids that are too close to avoid colliding
//function avoidOthers(boid)
//{
//    const minDistance = 20; // The distance to stay away from other boids
//    const avoidFactor = 0.05; // Adjust velocity by this %
//    let moveX = 0;
//    let moveY = 0;
//    for (let otherBoid of boids)
//    {
//        if (otherBoid !== boid)
//        {
//            if (distance(boid, otherBoid) < minDistance)
//            {
//                moveX += boid.x - otherBoid.x;
//                moveY += boid.y - otherBoid.y;
//            }
//        }
//    }

//    boid.dx += moveX * avoidFactor;
//    boid.dy += moveY * avoidFactor;
//}

//// Find the average velocity (speed and direction) of the other boids and
//// adjust velocity slightly to match.
//function matchVelocity(boid)
//{
//    const matchingFactor = 0.05; // Adjust by this % of average velocity

//    let avgDX = 0;
//    let avgDY = 0;
//    let numNeighbors = 0;

//    for (let otherBoid of boids)
//    {
//        if (distance(boid, otherBoid) < visualRange)
//        {
//            avgDX += otherBoid.dx;
//            avgDY += otherBoid.dy;
//            numNeighbors += 1;
//        }
//    }

//    if (numNeighbors)
//    {
//        avgDX = avgDX / numNeighbors;
//        avgDY = avgDY / numNeighbors;

//        boid.dx += (avgDX - boid.dx) * matchingFactor;
//        boid.dy += (avgDY - boid.dy) * matchingFactor;
//    }
//}

//// Speed will naturally vary in flocking behavior, but real animals can't go
//// arbitrarily fast.
//function limitSpeed(boid)
//{
//    const speedLimit = 15;

//    const speed = Math.sqrt(boid.dx * boid.dx + boid.dy * boid.dy);
//    if (speed > speedLimit)
//    {
//        boid.dx = (boid.dx / speed) * speedLimit;
//        boid.dy = (boid.dy / speed) * speedLimit;
//    }
//}

//const DRAW_TRAIL = false;

//function drawBoid(ctx, boid)
//{
//    const angle = Math.atan2(boid.dy, boid.dx);
//    ctx.translate(boid.x, boid.y);
//    ctx.rotate(angle);
//    ctx.translate(-boid.x, -boid.y);
//    ctx.fillStyle = "#558cf4";
//    ctx.beginPath();
//    ctx.moveTo(boid.x, boid.y);
//    ctx.lineTo(boid.x - 15, boid.y + 5);
//    ctx.lineTo(boid.x - 15, boid.y - 5);
//    ctx.lineTo(boid.x, boid.y);
//    ctx.fill();
//    ctx.setTransform(1, 0, 0, 1, 0, 0);

//    if (DRAW_TRAIL)
//    {
//        ctx.strokeStyle = "#558cf466";
//        ctx.beginPath();
//        ctx.moveTo(boid.history[0][0], boid.history[0][1]);
//        for (const point of boid.history) {
//    ctx.lineTo(point[0], point[1]);
//}
//ctx.stroke();
//  }
//}

//// Main animation loop
//function animationLoop()
//{
//    // Update each boid
//    for (let boid of boids)
//    {
//        // Update the velocities according to each rule
//        flyTowardsCenter(boid);
//        avoidOthers(boid);
//        matchVelocity(boid);
//        limitSpeed(boid);
//        keepWithinBounds(boid);

//        // Update the position based on the current velocity
//        boid.x += boid.dx;
//        boid.y += boid.dy;
//        boid.history.push([boid.x, boid.y])
//    boid.history = boid.history.slice(-50);
//  }

//  // Clear the canvas and redraw all the boids in their current positions
//  const ctx = document.getElementById("boids").getContext("2d");
//ctx.clearRect(0, 0, width, height);
//for (let boid of boids)
//{
//    drawBoid(ctx, boid);
//}

//// Schedule the next frame
//window.requestAnimationFrame(animationLoop);
//}

//window.onload = () => {
//    // Make sure the canvas always fills the whole window
//    window.addEventListener("resize", sizeCanvas, false);
//    sizeCanvas();

//    // Randomly distribute the boids to start
//    initBoids();

//    // Schedule the main animation loop
//    window.requestAnimationFrame(animationLoop);
//};