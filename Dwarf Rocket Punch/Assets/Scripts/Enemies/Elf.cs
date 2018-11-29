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
    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        groundCheck = this.transform.Find("GroundCheck");

        targetCount = 1;
        target = patrolPoints[1];
        current = patrolPoints[0];
        transform.position = current.position;
        
    }

    // Update is called once per frame
    protected virtual void Update() {
        //Debug.Log(current.gameObject.name);
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

    //Elf will patrol an area following pathing points
    private void Patrolling() {
        //Debug.Log("Current Move Direction: " + moveDirection.x);
        //Debug.Log("Current Target is " + target.name);
        if ((moveDirection.normalized.x > 0 && moveDirection.x < minDistance) || (moveDirection.normalized.x < 0 && moveDirection.x > (minDistance * -1))){
            //Debug.Log("I need to change target");
            if (targetCount == patrolPoints.Length - 1) {
                //Debug.Log("Going back to start");
                targetCount = 0;
            }
            else {
                targetCount++;
                //Debug.Log("next target num: " + targetCount);
            }
            current = target;
            areaChecked = false;
            target = patrolPoints[targetCount];
            //Debug.Log("My new Target is: " + target.name);
        }
        //Debug.Log(moveDirection.normalized);
        transform.GetComponent<Rigidbody2D>().velocity = moveDirection.normalized * patrolSpeed;
        //Debug.Log(transform.GetComponent<Rigidbody2D>().velocity);
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