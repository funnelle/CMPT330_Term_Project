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

    public Transform[] patrolPoints;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private AudioSource audioSource;
    private Transform groundCheck;
    private Transform current, target;
    private int endT;
    private float t, dT;

    // Use this for initialization
    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        groundCheck = this.transform.Find("GroundCheck");

        endT = 1;
        target = patrolPoints[1];
        current = patrolPoints[0];
        transform.position = current.position;
        dT = patrolSpeed / Vector2.Distance(current.position, target.position);
    }

    // Update is called once per frame
    void Update() {
        Pathing();
    }

    void Pathing() {
        t += Time.deltaTime * dT;

        if (t >= endT) {
            current = target;
            endT = (int)Mathf.Floor(t) + 1;
            target = patrolPoints[endT % patrolPoints.Length];

            dT = patrolSpeed / Vector2.Distance(current.position, target.position);
        }

        float left = t - endT + 1;
        transform.position = Vector2.Lerp(current.position, target.position, left);
    }

    /*
    void isGrounded() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
    }
    */
}
