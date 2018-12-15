using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controls the movement of the dwarf, including jumping. It also handles
/// the rocket jump explosions and the flipping of the dwarf.
/// </summary>
/// 
/// Variables
/// 
/// Punch Speed + Range
/// punchRange               How far away the dwarf can be from wall/ground for the punch to hit
/// punchDelay               How long dwarf must wait between punches

/// Explosion Size + Force
/// explosionRadius          Radius of the rocket explosion
/// explosionForce           Force applied to any object insde the explosion
///
/// Firing Variables, Raycasts and Checks
/// timeSinceFire            Time since last punch
/// hitCheck                 RayCastHit of rocket punch
/// dwarfPunch               Ray from dwarfPunchOrigin to where the dwarf aims at
/// dwarfPunchOrigin         Origin of dwarfPunch
/// explosionPos             Where the explosion originates
/// ground                   LayerMask of what is considered ground

/// Mouse Tracking Variables
/// mouseLocation            The mouse's current location
/// armDirection             Direction of the dwarf's arm
/// directionModifier        Modifies the direction of the arm based on which way the dwarf is facing
/// dwarfArm                 Transform of the arm

/// Dwarf Movement Variables
/// allowMovement            Boolean determining if the dwarf can move
/// onGround                 Boolean determining if the dwarf is on ground
/// isRocketJumping          Boolean determining if the dwarf is rocket jumping
/// allowWallJump            Boolean determining if the dwarf can wall jump
/// groundCheckRadius        Radius of OverlapCircel for checking if the dwarf is touching ground
/// maxSpeed                 Max velocity of the dwarf
/// airSpeed                 Speed modifier when in the air
/// jumpForce                Force applied to the dwarf when jumping
/// slideTime                How long the dwarf can slide on a wall
/// rb2d                     Rigidbody2D of the dwarf
/// groundCheck              Transform position of OverlapCircle

/// Dwarf Animation Variables
/// mainAnimator             Reference to animator that controls the whole dwarf
/// armAnimator              Reference to animator that controls the arm
/// facingRight              Boolean determining if the dwarf is facing right or not
/// movementSpeed            How fast the dwarf is moving

/// Particle Systems
/// explosionPS              Particle System of explosions
/// cartridgePS              Particle System of cartridges
/// 
/// Author: Evan Funnell        (EVF)
/// Editor: Eamonn McCormick    (EPM)
/// Editor: Eric Walker         (EW)
/// Editor: Ryan Dykstra        (RJD)
/// 

public class DwarfController : MonoBehaviour
{
    //Punch Speed + Range
    public float punchRange = 20f;
    public float punchDelay = 0f;

    //Explosion Size + Force
    public float explosionRadius = 50f;
    public float explosionForce = 25f;

    //Firing Variables, Raycasts and Checks
    private float timeSinceFire;
    private RaycastHit2D hitCheck;
    private Ray2D dwarfPunch;
    private Transform dwarfPunchOrigin;
    private Vector2 explosionPos;
    public LayerMask ground;

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

    //Dwarf Animation Variables
    private Animator mainAnimator;
    private Animator armAnimator;
    private bool facingRight;
    private float movementSpeed;

    //Particle Systems
    public ParticleSystem explosionPS;
    public ParticleSystem cartridgePS;

    /// <summary>
    /// Initialize variables at game start
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Initialized variables
    /// 2018-12-2   EPM     Added particle system instantiation
    /// 
    void Start()
    {

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
    /// 2018-11-20  RJD     Added wall jump code
    /// 
    void Update()
    {
        //Calculate movement speed based on axis input
        movementSpeed = Input.GetAxis("Horizontal");
        //Check if onGround using overlapCircle
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);

        //If on ground, dwarf is controlled by velocity
        if (onGround)
        {
            rb2d.velocity = new Vector2(movementSpeed * maxSpeed, rb2d.velocity.y);
        }
        //If in air, dwarf is controlled by adding forces
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
        if (Input.GetMouseButtonUp(0))
        {
            armAnimator.Play("Dwarf_arm_blast_1");
        }

        //Debug.Log(explosionPS);
    }

    /// <summary>
    /// Checks for collisions with the ground to ell if player is 'grounded'
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Added ground check sphere
    /// 2018-11-20  RJD     Added wall jump code
    /// 
    void FixedUpdate()
    {
        //If the player decides to jump...
        if (Input.GetButtonDown("Jump"))
        {
            //If grounded, apply a jump force
            if (onGround)
            {
                //Set y velocity to 0 and add jumping force
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            //If wall jumping, set velocity to 0 and add a force to impulse dwarf off the wall
            //depending on facing right or left
            else if (allowWallJump)
            {
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

        //Animate the dwarf to run if x velocity is > 0
        if (Mathf.Abs(rb2d.velocity.x) > 0.01f)
        {
            mainAnimator.SetBool("isRunning", true);
        }
        else
        {
            mainAnimator.SetBool("isRunning", false);
        }

        //Flip the dwarf depending on his x velocity
        if (rb2d.velocity.x < 0 && !facingRight)
        {
            Flip();
        }
        else if (rb2d.velocity.x > 0 && facingRight)
        {
            Flip();
        }
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
        //Dwarf is in the air
        onGround = false;
        //Reset punch time
        timeSinceFire = 0f;

        //Get location and direction of punch
        dwarfPunch.origin = dwarfPunchOrigin.position;
        dwarfPunch.direction = facingRight ? -armDirection : dwarfPunch.direction = armDirection;

        //Raycast the punch and check if it hit something
        hitCheck = Physics2D.Raycast(dwarfPunch.origin, dwarfPunch.direction, punchRange);
        if (hitCheck.collider != null)
        {
            //Set explosion point to the raycast's hit
            print("We've hit something");
            explosionPos = hitCheck.point;

            //Get colliders in the explosion radius and apply explosion force to them
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
            foreach (Collider2D hit in hitObjects)
            {   
                Rigidbody2D expVictim = hit.GetComponent<Rigidbody2D>();
                if(hit.gameObject.tag == "Elf" ) //Kill the elf!
                {
                    Destroy(hit.gameObject); //Just pop him. Obviously you'd want an animation to play typically.
                    continue;
                }
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
    /// 
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
        //Get direction that victim will be pushed by explosion
        Vector2 ExpDir = (Vector2)expVictim.transform.position - explosionPos;
        ExpDir = ExpDir.normalized;
        //Get distance between victim and explosion
        float explosionDistance = Vector2.Distance(explosionPos, ExpDir);
        //Debug.Log(explosionDistance);
        float explosionStrength = 1f;
        //Set velocity of victim to direction times strength of explosion
        expVictim.velocity = (ExpDir * (explosionStrength * explosionForce));

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
    /// 
    /// <param name="collision"> A collider </param>
    /// 
    /// 2018-11-20 RJD      Added wall jump code
    /// 
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Allow dwarf to wall jump and set his x velocity to 0, then start the cancelWallJump coroutine
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
    /// 
    /// <param name="collision"> A collider </param>
    /// 
    /// 2018-11-20      Added wall jump code
    /// 
    void OnCollisionExit2D(Collision2D collision)
    {
        //Disallow wall jump if dwarf is not touching a wall
        if (collision.gameObject.tag == "Wall")
            allowWallJump = false;
    }

    /// <summary>
    /// Coroutine to take away the ability to wall jump after sticking to a wall for a period of time
    /// </summary>
    /// 
    /// <param name="slideTime"> The amount of time before wall jump is disabled </param>
    /// 
    /// <returns> A WaitForSeconds() </returns>
    /// 
    /// 2018-11-20      Added basic wall jump cancelling
    /// 
    private IEnumerator cancelWallJump(float slideTime)
    {
        yield return new WaitForSeconds(slideTime);
        allowWallJump = false;
    }
}