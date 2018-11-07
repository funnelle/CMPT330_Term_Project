using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfRocketFire : MonoBehaviour
{

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

    // Update is called once per frame
    void Update()
    {

        timeSinceFire += Time.deltaTime;
        if (Input.GetButton("Fire1") && timeSinceFire >= punchDelay)
        {
            Punch();
        }
    }
    void Punch()
    {
        timeSinceFire = 0f;
        //reset our punch time
        dwarfPunch.origin = transform.position;
        dwarfPunch.direction = transform.forward;
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
    /// This function calculates the direction of force to be applied based on the explosion 
    /// </summary>
    /// <param name="expVictim"></param>
    /// <param name="explosionForce"></param>
    /// <param name="explosionPos"></param>
    /// <param name="explosionRadius"></param>
    void DwarfExplode(Rigidbody2D expVictim, float explosionForce, Vector2 explosionPos, float explosionRadius)
    {
        Vector2 ExpDir = (Vector2)expVictim.transform.position - explosionPos;
        //ExpDir = ExpDir.normalized;
        float explosionDistance = Vector2.Distance(explosionPos, ExpDir);
        float explosionStrength = 1f - (explosionDistance / explosionRadius);

        expVictim.AddForce(ExpDir * (explosionStrength * explosionForce), ForceMode2D.Impulse);
        //Impulse applies force instantly. Better suited for explosions.
    }
}