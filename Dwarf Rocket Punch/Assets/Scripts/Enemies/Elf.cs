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

    // Use this for initialization
    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        groundCheck = this.transform.Find("GroundCheck");
    }

    // Update is called once per frame
    void Update() {

    }

    void FixedUpdate() {

    }

    void isGrounded() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
    }
}
