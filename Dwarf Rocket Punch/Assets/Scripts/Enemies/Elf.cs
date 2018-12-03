using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Elf : MonoBehaviour {
    [SerializeField] private bool grounded;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float patrolCheckTime = 1f;
    [SerializeField] private float investigateCheckTime = 2f;
    [SerializeField] private float minDistance = 0.3f;
    [SerializeField] private float detectionAngle = 90f;

    public Transform[] patrolPoints;
    public Transform playerPosition;
    public GameObject positionMarker;

    protected enum State {PATROLLING, ATTACKING, SEARCHING, RETREATING};
    protected State state;
    protected bool facingRight = true;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private AudioSource audioSource;
    private Transform groundCheck;
    private Transform current, target;

    private int targetCount;
    private float elfPlayerDot;
    private bool checkingArea = false;
    private bool areaChecked = false;
    private bool playerSpotted = false;
    private bool lastKnownPlayerPositionSampled = false;
    private bool huntingPlayer = false; 
    private Vector3 moveDirection;
    private Vector2 playerDirection;
    private Vector3 lastKnownPlayerPosition;
    private Vector2 lastKnownPlayerDirection;

    // Use this for initialization
    protected virtual void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        groundCheck = this.transform.Find("GroundCheck");
        state = State.PATROLLING;

        targetCount = 1;
        target = patrolPoints[1];
        current = patrolPoints[0];
        transform.position = current.position;
        
    }

    // Update is called once per frame
    protected virtual void Update() {
        Debug.Log("My Current State: " + state);
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
            Debug.Log("I'm here bitch, fuck you elf");
            lastKnownPlayerDirection = positionMarker.transform.position - transform.position;
            lastKnownPlayerDirection.y = 0;

            if ((lastKnownPlayerDirection.normalized.x > 0 && lastKnownPlayerDirection.x < minDistance) || (lastKnownPlayerDirection.normalized.x < 0 && lastKnownPlayerDirection.x > (minDistance * -1))) {
                Debug.Log("I'm already tracer");
                transform.position = new Vector2(positionMarker.transform.position.x, transform.position.y);
                StartCoroutine(InvestigationCheck(investigateCheckTime));
            }

            if (IsGrounded()) {
                rb2d.velocity = new Vector2(lastKnownPlayerDirection.normalized.x * patrolSpeed, 0);
            }
            else {
                rb2d.velocity = new Vector2(lastKnownPlayerDirection.normalized.x * patrolSpeed, rb2d.velocity.y);
            }
            Debug.Log("my velocity " + rb2d.velocity);
        }

        //Player detection
        playerDirection = playerPosition.position - transform.position;
        if (facingRight) {
            elfPlayerDot = Vector2.Angle(playerDirection, transform.right);
        }
        else if (!facingRight) {
            elfPlayerDot = Vector2.Angle(playerDirection, (transform.right * -1));
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection);
        Debug.DrawRay(transform.position, playerDirection, Color.red);

        if (elfPlayerDot < detectionAngle * 0.5f) {
            Debug.Log("I see you");
            Debug.Log(hit.collider.name);
            Debug.Log(playerPosition.name);
            if (hit.collider.name != playerPosition.parent.name) {
                Debug.Log("Why you behind a wall, boo you suck");
                state = State.PATROLLING;
            }
            else {
                playerSpotted = true;
                state = State.ATTACKING;
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
    }

    //Elf will patrol an area following pathing points
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

    private IEnumerator PatrolCheck(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        checkingArea = false;
        areaChecked = true;
    }

    private IEnumerator InvestigationCheck(float investigateTime) {
        yield return new WaitForSeconds(investigateTime);
        playerSpotted = false;
        lastKnownPlayerPositionSampled = false;
        state = State.PATROLLING;
    }

    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    bool IsGrounded() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
        return grounded;
    }
}