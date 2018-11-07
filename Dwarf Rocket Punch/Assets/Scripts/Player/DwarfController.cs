using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls Dwarf (Player) movement and ground check
/// </summary>
/// 
/// Field               Description
/// *public*
/// allowMovement       Boolean value that determines if the Dwarf is allowed to move
/// onGround            Boolean value that determines if the Dwarf is on the ground
/// groundCheckRadius   Radius of OverlapCircle for checking if touching ground
/// maxSpeed            Speed of Dwarf's movement
/// ground              LayerMask to determine what should be considered 'Ground'
/// 
/// *private*
/// rb2d                Rigidbody2D of Dwarf body
/// movementSpeed       Axis input value between -1 and 1 
/// groundCheck         Transform position of OverlapCircle
/// 
/// Author: Evan Funnell (EVF)
/// 
public class DwarfController : MonoBehaviour {
    public bool allowMovement = true;
    public bool onGround = true;
    public float groundCheckRadius = 0.1f;
    public float maxSpeed = 10f;
    public LayerMask ground;

    private Rigidbody2D rb2d;
    private Transform groundCheck;
    private Animator mainAnimator;
    private bool facingRight;
    private float movementSpeed;

    /// <summary>
    /// Initialize variables at game start
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Initialized variables
    /// 
    void Start() {
        rb2d = this.GetComponent<Rigidbody2D>();
        groundCheck = GameObject.Find("/Dwarf/GroundCheck").GetComponent<Transform>();
        mainAnimator = GameObject.Find("/Dwarf/MainAnimationRig").GetComponent<Animator>();
    }

    /// <summary>
    /// Gets input and moves player
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Added movement code
    /// 
    void Update() {
        if (allowMovement) {
            movementSpeed = Input.GetAxis("Horizontal");
            rb2d.velocity = new Vector2(movementSpeed * maxSpeed, rb2d.velocity.y);
        }
    }

    /// <summary>
    /// Checks for collisions with the ground to ell if player is 'grounded'
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Added ground check sphere
    /// 
    void FixedUpdate() {
        //create a sphere that checks if we are on ground
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);

        if (Mathf.Abs(rb2d.velocity.x) > 0.01f) {
            mainAnimator.SetBool("isRunning", true);
        }
        else {
            mainAnimator.SetBool("isRunning", false);
        }

        if (rb2d.velocity.x < 0 && !facingRight) {
            Flip();
        }
        else if (rb2d.velocity.x > 0 && facingRight) {
            Flip();
        }
    }

    //Debug function to test OnGround check
    void OnDrawGizmosSelected() {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

    /// <summary>
    /// FLips the render of the character when the velocity parity flips (the player turns around)
    /// </summary>
    /// 
    /// 2018-11-7 EPM       Added flip code
    /// 
    void Flip() {
        facingRight = !facingRight;
        //flips parity of x-axis render, flipping the character around
        Vector3 mainScale = transform.localScale;
        mainScale.x *= -1;
        transform.localScale = mainScale;
        //flips parity of mouse track vector so that shoulder does not track mouse when player turns
        TrackMouse.directionModifier *= -1;
    }
}