using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for all elf enemies.  All functions here are used by all elf
/// enemies in the game, with enemy types (i.e. archers) inheriting from this class.
/// This class controls the finite state machine that controls the enemy, determining whether
/// they should be patrolling a given area, attacking the player, or investigating that last
/// known position of the player
/// </summary>
/// 
/// Field                       Description
/// *SerializeField*
/// grounded                    Bool value that is true if the elf is currently grounded
/// groundCheckRadius           Float value that decides the radius of the GroundCheck overlap circle
/// ground                      Layermask of the layers that count as ground
/// patrolSpeed                 Speed the Elf moves while patrolling
/// patrolCheckTime             Amount of time the elf spends checking his surroundings at each end node in his patrol
/// minDistance                 Distance from a point before the elf's transform is set to the position of that point
/// detectionAngle              Degrees of vision the elf has in their player detection cone
/// visionDistance              How far away from him the elf can see 
/// 
/// *public*
/// dead                        Bool value that says if the elf has died or not
/// patrolPoints                List of Transform points the elf will use to patrol between.  Must contain a point called Start and another called End
/// playerPosition              Transform of the player's animation rig that gives the current position of the player for tracking
/// positionMarker              GameObject used to mark the last scene position of the player when the player leaves the elf's vision so the elf can investgate that spot
/// 
/// *protected*
/// State                       Enum that holds all of the possible states the elf can transition between
/// state                       State variable that holds the currently active state
/// facingRight                 Bool variable that is true if the elf sprite is facing right
/// mainAnimator                Reference to Animator for the main body
/// armAnimator                 Reference to Animator for the arms
/// arrowParticle               Reference to ParticleSystem for creating arrows
/// 
/// *private*
/// rb2d                        Reference to the Rigidbody2D component
/// groundCheck                 Reference to the GroundCheck child object of the elf
/// current                     Transform of the current patrol node
/// target                      Transform of the target patrol node
/// targetCount                 Int that counts which target in the list we are currently at. Used to loop back to the start
/// elfPlayerAngle              Float that gives the angle the player is away from the given direction
/// checkingArea                Bool that is true if the elf is currently checking the area at the end of a patrol
/// areaChecked                 Bool that is true if the elf successfully checked the area. Prevents rechecking
/// playerSpotted               Bool that is true if Player is in range and raycast sees them
/// lastKnownPositionSampled    Bool that is true if we have grabbed the player's last known position
/// moveDirection               Vector3 that gives the direction to move to get to the next point
/// playerDirection             Vector2 that tracks the relative location of the player to see if they are in range
/// lastKnownPlayerDirection    Vector2 that holds the relative location of the last known player position
/// 
/// Author: Evan Funnell    EVF
/// 
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Elf : MonoBehaviour {
    [SerializeField] private bool grounded;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float patrolCheckTime = 1f;
    [SerializeField] private float investigateCheckTime = 2f;
    [SerializeField] private float minDistance = 0.2f;
    [SerializeField] private float detectionAngle = 90f;
    [SerializeField] private float visionHysteresis = 2f;
    [SerializeField] private float visionDistance = 10f;

    public bool dead = false;
    public Transform[] patrolPoints;
    public Transform playerPosition;
    public GameObject positionMarker;
    public GameObject mainAnimatorGO;
    public GameObject armAnimatorGO;

    protected enum State {PATROLLING, ATTACKING, SEARCHING};
    protected State state;
    protected bool facingRight = true;

    private Rigidbody2D rb2d;
    private AudioSource audioSource;
    private Transform groundCheck;
    private Transform current, target;

    private int targetCount;
    private float elfPlayerAngle;
    private bool checkingArea = false;
    private bool areaChecked = false;
    private bool playerSpotted = false;
    private bool lastKnownPlayerPositionSampled = false;
    private Vector3 moveDirection;
    private Vector2 playerDirection;
    private Vector2 lastKnownPlayerDirection;

    //animation controllers
    protected Animator mainAnimator;
    protected Animator armAnimator;
    //particle system
    protected ParticleSystem arrowParticle;

    /// <summary>
    /// Initializes some components, sets a base state, and sets the first patrol points.
    /// Protected Virtual function so classes that inherit from this one can use start
    /// </summary>
    /// 
    /// 2018-12-01  EVF     Initial State
    /// 
    protected virtual void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        groundCheck = this.transform.Find("GroundCheck");
        state = State.PATROLLING;

        targetCount = 1;
        target = patrolPoints[1];
        current = patrolPoints[0];
        transform.position = current.position;

        //grab animators
        mainAnimator = mainAnimatorGO.GetComponent<Animator>();
        armAnimator = armAnimatorGO.GetComponent<Animator>();

    }

    /// <summary>
    /// Controls what happens during each finite state and detects the player and whether they are able to be attacked.
    /// protected virtual function so classes that inherit from this one can use update
    /// </summary>
    /// 
    /// 2018-12-01  EVF     Added finite state machine
    /// 
    protected virtual void Update() {
        //Debug.Log("My Current State: " + state);
        //Patrolling State
        if (state == State.PATROLLING) {
            moveDirection = target.position - transform.position;
            moveDirection.y = 0;

            if (current.gameObject.name == "Start" || current.gameObject.name == "End") {
                if (!areaChecked && !checkingArea) {
                    checkingArea = true;
                    StartCoroutine(PatrolCheck(patrolCheckTime));
                }
                if (areaChecked) {
                    Patrolling();
                }
            }
            else {
                areaChecked = false;
                Patrolling();
            }
        }

        //SEARCHING state
        if (state == State.SEARCHING) {
            lastKnownPlayerDirection = positionMarker.transform.position - transform.position;
            lastKnownPlayerDirection.y = 0;

            if ((lastKnownPlayerDirection.normalized.x > 0 && lastKnownPlayerDirection.x < minDistance) || (lastKnownPlayerDirection.normalized.x < 0 && lastKnownPlayerDirection.x > (minDistance * -1))) {
                transform.position = new Vector2(positionMarker.transform.position.x, transform.position.y);
                StartCoroutine(InvestigationCheck(investigateCheckTime));
            }

            if (IsGrounded()) {
                rb2d.velocity = new Vector2(lastKnownPlayerDirection.normalized.x * patrolSpeed, 0);
            }
            else {
                rb2d.velocity = new Vector2(lastKnownPlayerDirection.normalized.x * patrolSpeed, rb2d.velocity.y);
            }
        }

        //Player detection
        playerDirection = playerPosition.position - transform.position;
        if (facingRight) {
            if (playerSpotted) {
                Vector2 followPlayer = Vector2.Lerp(transform.right, playerDirection, visionHysteresis * Time.deltaTime);
                elfPlayerAngle = Vector2.Angle(playerDirection, followPlayer);
            } else {
                elfPlayerAngle = Vector2.Angle(playerDirection, transform.right);
            }
        }
        else if (!facingRight) {
            if (playerSpotted) {
                Vector2 followPlayer = Vector2.Lerp((transform.right * -1), playerDirection, visionHysteresis * Time.deltaTime);
                elfPlayerAngle = Vector2.Angle(playerDirection, followPlayer);
            }
            else {
                elfPlayerAngle = Vector2.Angle(playerDirection, (transform.right * -1));
            }
        }

        //Shoots raycast to determine if player is visible or behind something
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection);
        Debug.DrawRay(transform.position, playerDirection, Color.red);
        if (playerDirection.x <= visionDistance && playerDirection.y <= visionDistance) {
            //if player angle is less than half the detection angle, player is visible, but may still be behind a wall
            if (elfPlayerAngle < detectionAngle * 0.5f) {
                //checks if the player is behind a wall or visible
                if (hit.collider.name != playerPosition.parent.name) {
                    state = State.PATROLLING;
                }
                else {
                    playerSpotted = true;
                    state = State.ATTACKING;
                }
            }
        }
      
        //Identify last known position and start searching
        if (playerSpotted && (hit.collider.name != playerPosition.parent.name)) {
            state = State.SEARCHING;
            if (lastKnownPlayerPositionSampled == false) {
                positionMarker.transform.position = playerPosition.position;
                lastKnownPlayerPositionSampled = true;
            }
        }

        //animations
        if (Mathf.Abs(rb2d.velocity.x) > 0f) {
            mainAnimator.SetBool("isRunning", true);
            armAnimator.SetBool("isRunning", true);
        }
        else {
            mainAnimator.SetBool("isRunning", false);
            armAnimator.SetBool("isRunning", false);
        }
    }

    /// <summary>
    /// This function controls the elf's movement between each target.  If the targetCount reaches the length
    /// of the patrolPoints array, its reset to 0 to form a cyclical patrol pattern.
    /// </summary>
    /// 
    /// 2018-12-01  EVF     Added velocity based cyclic patrolling
    /// 
    private void Patrolling() {
        if ((moveDirection.normalized.x > 0 && moveDirection.x < minDistance) || (moveDirection.normalized.x < 0 && moveDirection.x > (minDistance * -1))){
            if (targetCount == patrolPoints.Length - 1) {
                targetCount = 0;
            }
            else {
                targetCount++;
            }
            current = target;
            areaChecked = false;
            target = patrolPoints[targetCount];
        }

        if (IsGrounded()) {
            rb2d.velocity = new Vector2(moveDirection.normalized.x * patrolSpeed, 0);
        }
        else {
            rb2d.velocity = new Vector2(moveDirection.normalized.x * patrolSpeed, rb2d.velocity.y);
        }

        if (moveDirection.normalized.x > 0 && !facingRight) {
            Flip();
        } else if (moveDirection.normalized.x < 0 && facingRight) {
            Flip();
        }

    }

    /// <summary>
    /// When the elf reaches a start or end node, wait a few seconds before proceeding to patrol
    /// </summary>
    /// <param name="waitTime">Time to wait at the point</param>
    /// 
    /// 2018-12-01  EVF     Initial State
    /// 
    private IEnumerator PatrolCheck(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        checkingArea = false;
        areaChecked = true;
    }

    /// <summary>
    /// When the elf loses sight of the player, wait at the last known position to search for player.
    /// Then return to patrolling
    /// </summary>
    /// <param name="investigateTime">Time to wait to investigate area</param>
    /// 
    /// 2018-12-01  EVF     Initial State
    /// 
    private IEnumerator InvestigationCheck(float investigateTime) {
        yield return new WaitForSeconds(investigateTime);
        playerSpotted = false;
        lastKnownPlayerPositionSampled = false;
        state = State.PATROLLING;
    }

    /// <summary>
    /// Flips the local scale of the game object to face the other direction
    /// </summary>
    /// 
    /// 2018-12-01  EVF     Added local scale flip
    /// 
    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    /// <summary>
    /// Checks if the elf is on the ground
    /// </summary>
    /// <returns><c>true</c>if elf is on the ground<c>false</c>if elf is not on ground</returns>
    /// 
    /// 2018-12-01  EVF     Added ground check
    /// 
    bool IsGrounded() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
        return grounded;
    }

    /// <summary>
    /// Called if the elf is hit by an explosion
    /// </summary>
    public void ElfHit() {
        dead = true;
        //Debug.Log("Im dead, ah shit, I'll get you next time");
    }
}