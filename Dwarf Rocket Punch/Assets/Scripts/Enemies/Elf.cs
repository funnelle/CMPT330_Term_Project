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
    [SerializeField] private float minDistance = 0.3f;
    [SerializeField] private float detectionAngle = 90f;

    public Transform[] patrolPoints;
    public Transform playerPosition;

    protected enum State {PATROLLING, ATTACKING, RETREATING};
    protected State state;
    protected bool facingRight = true;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private AudioSource audioSource;
    private Transform groundCheck;
    private Transform current, target;
    private int targetCount;
    private bool checkingArea = false;
    private bool areaChecked = false;
    private Vector3 moveDirection;
    private float elfPlayerDot;
    protected Vector2 playerDirection;
    //animation controllers
    protected Animator mainAnimator;
    protected Animator armAnimator;
    //particle system
    protected ParticleSystem arrowParticle;

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

        //grab animators
        mainAnimator = GameObject.Find("ElfAnimationRig").GetComponent<Animator>();
        armAnimator = GameObject.Find("ElfAnimationRig/Elf_torso/Elf_arms/ElfArmAnimationController").GetComponent<Animator>();
        //grab particle system
        arrowParticle = GameObject.Find("ElfAnimationRig/Elf_torso/Elf_arms/ElfArmAnimationController").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    protected virtual void Update() {
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
                state = State.ATTACKING;
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
        transform.GetComponent<Rigidbody2D>().velocity = moveDirection.normalized * patrolSpeed;

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

    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void isGrounded() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
    }


}