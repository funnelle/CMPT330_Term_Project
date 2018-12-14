using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls Dwarf (Player) movement and ground check
/// </summary>
/// 
/// Field               Description
/// 
/// Punch Speed + Range
/// punchRange          How far away from ground/wall the punch can be
/// punchDelay          Delay between each punch
/// 
/// Explosive Size + Force
/// explosionRadius     Radius of the rocket explosion
/// explosionForce      Force applied to objects inside the explosion
/// 
/// Firing Variables, Raycasts, and Checks
/// ground              LayerMask specifying what is ground for groundCheck
/// timeSinceFire       Time since last punch
/// hitCheck            RayCastHit at point of dwarf punch
/// dwarfPunch          Ray originating from dwarfPunchOrigin to some point
/// dwarfPunchOrigin    Transform of where dwarfPunch will begin
/// explosionPos        Vector2 point where a rocket explosion happens
/// 
/// Mouse Tracking Variables
/// mouseLocation       Location of mouse
/// armDirection        Direction of dwarf arms
/// direcitonModifier   Modifies the arm direction based on the dwarf's direction
/// dwarfArm            Transform of the dwarf's arm
/// 
/// Dwarf Movemement Variables
/// allowMovement       Boolean determining if the dwarf can move
/// onGround            Boolean determining if the dwarf is on ground
/// isRocketJumping     Boolean determining if the dwarf is rocket jumping
/// allowWallJump       Boolean determining if the dwarf can wall jump
/// groundCheckRadius   Radius of OverlapCircle for checking if dwarf is touching the ground
/// maxSpeed            Max velocity of the dwarf's movement
/// airSpeed            Speed modifier of the dwarf while in air
/// jumpForce           Force of the dwarf's jumps
/// slideTime           How long the dwarf can slide down a wall while still being allowed to wall jump
/// rb2d                RigidBody2d of the dwarf's body
/// groundCheck         Transform position of OverlapCircle
/// 
/// Dwarf Animation Variables
/// mainAnimator        Reference to the main animator of the dwarf
/// armAnimator         Reference to the animator of the dwarf's arm
/// facingRight         Boolean determining if the dwarf is facing right or not
/// movementSpeed       Current movement speed of the dwarf
/// 
/// Particle Systems
/// explosionPS         Particle System of explosions
/// cartridgePS         Particle System of cartridges
/// 
/// Author: Evan Funnell        (EVF)
/// Editor: Eamonn McCormick    (EPM)
/// Editor: Ryan Dykstra        (RJD)
/// Editor: Eric Walker         (EW)

public class DwarfController : MonoBehaviour {

    //Punch speed + range
    public float punchRange = 20f;
    public float punchDelay = 0f;

    //Explosion size + force
    public float explosionRadius = 50f;
    public float explosionForce = 25f;

    //Firing variables, raycasts and checks
    public LayerMask ground;
    private float timeSinceFire;
    private RaycastHit2D hitCheck;
    private Ray2D dwarfPunch;
    private Transform dwarfPunchOrigin;
    private Vector2 explosionPos;

    //Mouse Tracking Variables
    private Vector3 mouseLocation;
    public Vector2 armDirection;
    public static int directionModifier = 1;
    private Transform dwarfArm;

    //Dwarf Movement Variables
    public bool allowMovement = true;
    public bool onGround = true;
    public bool isRocketJumping;
    public bool allowWallJump = false;
    public float groundCheckRadius = 0.1f;
    public float maxSpeed = 10f;
    public float airSpeed = 0.28f;
    public float jumpForce = 15f;
    public float slideTime = 1f;
    private Rigidbody2D rb2d;
    private Transform groundCheck;

    //Dwarf animation variables
    private Animator mainAnimator;
    private Animator armAnimator;
    private bool facingRight;
    private float movementSpeed;

    //Particle systems
    public ParticleSystem explosionPS;
    public ParticleSystem cartridgePS;

    /// <summary>
    /// Initialize variables at game start
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Initialized variables
    /// 2018-12-2   EPM     Added particle system instantiation
    /// 
    void Start(){

        //Let his punches leave his own collider
        Physics2D.queriesStartInColliders = false;

        //Get the components we need for moving the player
        rb2d = this.GetComponent<Rigidbody2D>();
        groundCheck = GameObject.Find("/Dwarf/GroundCheck").GetComponent<Transform>();

        //Get the componenets we need for animation the player
        mainAnimator = GameObject.Find("/Dwarf/DwarfAnimationRig").GetComponent<Animator>();
        armAnimator = GameObject.Find("/Dwarf/DwarfAnimationRig/Torso/Arms/DwarfArmAnimationRig").GetComponent<Animator>();

        //Get the component we need to change with mouse direction
        dwarfArm = GameObject.Find("/Dwarf/DwarfAnimationRig/Torso/Arms").GetComponent<Transform>();

        //Get the component that is the origin of the dwarf's punch raycast.
        dwarfPunchOrigin = GameObject.Find("/Dwarf/RocketLocation/").GetComponent<Transform>();

        //Instantiate particle systems
        explosionPS = Instantiate(explosionPS);
        cartridgePS = Instantiate(cartridgePS);
    }
        


    /// <summary>
    /// Gets input and moves player
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Added movement code
    /// 2018-11-7   EPM     Added mouse click code
    /// 2018-12-14  RJD     Added movement code
    /// 
    void Update() {

        //Get movement speed from Input.GetAxis and update onGround if Dwarf is onGround
        movementSpeed = Input.GetAxis("Horizontal");
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);

        //If Dwarf is onGround, movement control is based on velocity
        if (onGround)
        {
            rb2d.velocity = new Vector2(movementSpeed * maxSpeed, rb2d.velocity.y);
        }

        //If in air, movement control is based on adding forces
        else if (!onGround)
        {
            rb2d.AddForce(new Vector2(movementSpeed * airSpeed, 0), ForceMode2D.Impulse);
            //If x velocity becomes greater than maxSpeed, set it to maxSpeed
            if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
                rb2d.velocity = new Vector2(movementSpeed * maxSpeed, rb2d.velocity.y);
        }

        //If the player is able to, allow them to punch
        timeSinceFire += Time.deltaTime;
        if (Input.GetButton("Fire1") && timeSinceFire >= punchDelay)
        {
            print("Fire!");
            Punch();
        }
        //Track the mouse and place the dwarfs arm towards it
        mouseLocation = Input.mousePosition;
        mouseLocation = Camera.main.ScreenToWorldPoint(mouseLocation);
        armDirection = new Vector2(mouseLocation.x - transform.position.x, mouseLocation.y - transform.position.y) * directionModifier;
        dwarfArm.right = armDirection;
        //play arm animation on click
        if (Input.GetMouseButtonUp(0)) {
            armAnimator.Play("Dwarf_arm_blast_1");
        }
        //armAnimator.SetBool("onClick", Input.GetMouseButtonUp(0));

        Debug.Log(explosionPS);
    }

    /// <summary>
    /// Checks for collisions with the ground to ell if player is 'grounded'
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Added ground check sphere
    /// 2018-12-14  RJD     Tuned up wall jumping code
    /// 
    void FixedUpdate()
    {
        //If the player jumps...
        if (Input.GetButtonDown("Jump"))
        {
            //If on ground, perfrom a simple jump by adding y force
            if (onGround)
            {
                //Set y velocity to 0 and add jumping force
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            //If performing a walljump, set velocity to 0 and add a force to impulse him off the wall
            //depending facing right or left
            else if (allowWallJump)
            {
                //Set velocity to 0 and add a force to impulse him off the wall depending facing right or left
                rb2d.velocity = Vector2.zero;
                if (!facingRight)
                {
                    rb2d.AddForce(new Vector2(-jumpForce / 2, jumpForce), ForceMode2D.Impulse);
                }
                if (facingRight)
                {
                    rb2d.AddForce(new Vector2(jumpForce / 2, jumpForce), ForceMode2D.Impulse);
                }
                allowWallJump = false;
            }
        }
        
        //Set the animator to make the dwarf run if its velocity is > 0
        if (Mathf.Abs(rb2d.velocity.x) > 0.01f) {
            mainAnimator.SetBool("isRunning", true);
        }
        else
        {
            mainAnimator.SetBool("isRunning", false);
        }

        //Flip the dwarf depending on the magnitude of x velocity
        if (rb2d.velocity.x < 0 && !facingRight)
        {
            Flip();
        }
        else if (rb2d.velocity.x > 0 && facingRight)
        {
            Flip();
        }
    }

    //Debug function to test OnGround check
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(explosionPos, explosionRadius);
    }

    /// <summary>
    /// FLips the render of the character when the velocity parity flips (the player turns around)
    /// </summary>
    /// 
    /// 2018-11-7   EPM     Added flip code
    /// 
    void Flip()
    {
        facingRight = !facingRight;

        //flips parity of x-axis render, flipping the character around
        Vector3 mainScale = transform.localScale;
        mainScale.x *= -1;
        transform.localScale = mainScale;
        //flips parity of mouse track vector so that shoulder does not track mouse when player turns
        directionModifier *= -1;
    }

    /// <summary>
    /// Fire a raycast in the direction of the dwarfs gauntlet, and create a circle (the explosion), and from there
    /// apply our explosion to each rigidbody that the circle has made contact with.
    /// </summary>
    /// 
    /// 2018-11-07  EW
    /// 
    void Punch()
    {
        onGround = false;
        timeSinceFire = 0f;
        //reset our punch time
        dwarfPunch.origin = dwarfPunchOrigin.position;
        dwarfPunch.direction = facingRight ? -armDirection : dwarfPunch.direction = armDirection;
        //Get our location
        hitCheck = Physics2D.Raycast(dwarfPunch.origin, dwarfPunch.direction, punchRange);
        if (hitCheck.collider != null)
        {
            print("We've hit something");
            explosionPos = hitCheck.point;
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
    /// 
    /// 2018-11-07  EW
    /// 2018-12-2   EPM     Added particle effect code
    /// 
    void DwarfExplode(Rigidbody2D expVictim, float explosionForce, Vector2 explosionPos, float explosionRadius)
    {
        Vector2 ExpDir = (Vector2)expVictim.transform.position - explosionPos;
        ExpDir = ExpDir.normalized;
        float explosionDistance = Vector2.Distance(explosionPos, ExpDir);
        Debug.Log(explosionDistance);
        float explosionStrength = 1f;

        expVictim.velocity = (ExpDir * (explosionStrength * explosionForce));
        //Tell the engine to simply shove them in our desired direction, no fuss.

        //play the particle effect
        //note: even though these PS's are part of the script, they do not move around with it, and must therefore be moved into
        //the correct position before being played
        explosionPS.transform.position = explosionPos;
        explosionPS.Play();
        cartridgePS.transform.position = dwarfArm.transform.position;
        cartridgePS.Play();
    }

    /// <summary>
    /// Performs actions after a collision
    /// </summary>
    /// <param name="collision"> A collider </param>
    /// 
    /// 2018-11-20  RJD     Added Wall-Jump Code
    /// 
    void OnCollisionEnter2D(Collision2D collision)
    {
        //If just jumped onto a wall, allow wall jump, set x velocity to 0, and start the
        //CancelWallJump coroutine
        if (collision.gameObject.tag == "Wall")
        {
            allowWallJump = true;
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            StopAllCoroutines();
            IEnumerator coroutine = cancelWallJump(slideTime);
            StartCoroutine(coroutine);
        }
    }

    /// <summary>
    /// Performs actions after ceasing contact with a collider
    /// </summary>
    /// <param name="collision"> A collider </param>
    /// 
    /// 2018-11-20 RJD    Added wall jump code
    /// 
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
            allowWallJump = false;
    }

    /// <summary>
    /// Coroutine to take away the ability to wall jump after sticking to a wall for a period of time
    /// </summary>
    /// <param name="slideTime"> The amount of time before wall jump is disabled </param>
    /// <returns> A WaitForSeconds() </returns>
    /// 
    /// 2018-11-20 RJD     Added basic wall jump cancelling
    /// 
    private IEnumerator cancelWallJump(float slideTime)
    {
        yield return new WaitForSeconds(slideTime);
        allowWallJump = false;
    }
}