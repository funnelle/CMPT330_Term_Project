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

    public Transform[] patrolPoints;

    protected enum State {PATROLLING, ATTACKING, RETREATING};
    protected State state;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private AudioSource audioSource;
    private Transform groundCheck;
    private Transform current, target;
    private int targetCount;
    private bool checkingArea = false;
    private bool areaChecked = false;
    private Vector3 moveDirection;

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
    }

    private IEnumerator PatrolCheck(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        checkingArea = false;
        areaChecked = true;
    }

    void isGrounded() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
    }
}