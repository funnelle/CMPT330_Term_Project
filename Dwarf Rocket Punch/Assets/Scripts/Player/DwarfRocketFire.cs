using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackMouse))]
public class DwarfRocketFire : MonoBehaviour
{
    private TrackMouse MouseTracker;
    //Punch speed + range
    public float punchRange = 5f;
    public float punchDelay = 0.15f;

    //Explosion size + force
    public float explosionRadius = 50f;
    public float explosionForce = 25f;
    public float dwarfUplift = 35f;

    //Firing variables, raycasts and checks
    private float timeSinceFire;
    private RaycastHit2D hitCheck;
    private Ray2D dwarfPunch;
    private LayerMask playerLayer = ~1 << 9;

    private Transform gauntletLocation;
    /// <summary>
    /// Find our gauntlet game objects location for use in our punch.
    /// </summary>
    /// EW 2018-11-07
    private void Start()
    {
        gauntletLocation = GameObject.Find("/Dwarf/MainAnimationRig/Torso/Arms/Gauntlet").GetComponent<Transform>();
        dwarfPunch.origin = gauntletLocation.position;
        Physics2D.queriesStartInColliders = false;
        MouseTracker = GetComponent<TrackMouse>();
    }

    /// <summary>
    /// Check if we're allowed to fire again, and if so, call the punch function.
    /// </summary>
    /// EW 2018-11-07
    void Update()
    {

        timeSinceFire += Time.deltaTime;
        if (Input.GetButton("Fire1") && timeSinceFire >= punchDelay)
        {
            print("Fire!");
            Punch();
        }
    }

    /// <summary>
    /// Fire a raycast in the direction of the dwarfs gauntlet, and create a circle (the explosion), and from there
    /// apply our explosion to each rigidbody that the circle has made contact with.
    /// </summary>
    /// EW 2018-11-07
    void Punch()
    {
        timeSinceFire = 0f;
        //reset our punch time
        dwarfPunch.origin = this.transform.position;
        dwarfPunch.direction = MouseTracker.armDirection;
        //Get our location
        hitCheck = Physics2D.Raycast(dwarfPunch.origin, dwarfPunch.direction, punchRange);
        if (hitCheck.collider != null)
        {
            print("We've hit something");
            Vector2 explosionPos = hitCheck.point;
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
            foreach (Collider2D hit in hitObjects)
            {
                Rigidbody2D expVictim = hit.GetComponent<Rigidbody2D>();
                if (expVictim != null)
                {
                    print("WE'VE HIT");
                    DwarfExplode(expVictim, explosionForce, explosionPos, explosionRadius);
                }
            }
        }
    }

    /// <summary>
    /// This function calculates the direction of force to be applied based on the explosions parameters. 
    /// It will send whichever rigidbody is supplied into the calculated direction at the specified force.
    /// </summary>
    /// <param name="expVictim">The Rigidbody 2D we wish to apply force to.</param>
    /// <param name="explosionForce">Desired strength of the explosion (a float).</param>
    /// <param name="explosionPos">The origin of the explosion (A vector2 location.)</param>
    /// <param name="explosionRadius">The width of the explosion. (A float.)</param>
    /// EW 2018-11-07
    void DwarfExplode(Rigidbody2D expVictim, float explosionForce, Vector2 explosionPos, float explosionRadius)
    {
        Vector2 ExpDir = (Vector2)expVictim.transform.position - explosionPos;
        ExpDir = ExpDir.normalized;
        float explosionDistance = Vector2.Distance(explosionPos, ExpDir);
        float explosionStrength = 1f - (explosionDistance / explosionRadius);

        expVictim.velocity=(ExpDir * (explosionStrength * explosionForce));
        //Tell the engine to simply shove them in our desired direction, no fuss.
    }
}