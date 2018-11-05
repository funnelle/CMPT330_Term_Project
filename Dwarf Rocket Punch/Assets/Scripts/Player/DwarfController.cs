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
public class DwarfController : MonoBehaviour
{
    public bool allowMovement = true;
    public bool onGround = true;
    public float groundCheckRadius = 0.1f;
    public float maxSpeed = 10f;
    public LayerMask ground;
    public float jumpSpeed = 15f;
    public bool allowWallJump = false;
    private Rigidbody2D rb2d;
    private float movementSpeed;
    private Transform groundCheck;
    /// <summary>
    /// Initialize variables at game start
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Initialized variables
    /// 
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        groundCheck = GameObject.Find("/Dwarf/GroundCheck").GetComponent<Transform>();
    }
    /// <summary>
    /// Gets input and moves player
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Added movement code
    /// 2018-11-04  RJD     Added jump code
    /// 
    void Update()
    {
        if (allowMovement)
        {
            movementSpeed = Input.GetAxis("Horizontal");
            rb2d.velocity = new Vector2(movementSpeed * maxSpeed, rb2d.velocity.y);
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (onGround == true)
            {
                rb2d.velocity += new Vector2(0, jumpSpeed);
            }
            if (allowWallJump == true)
            {
                rb2d.velocity = new Vector2(0, jumpSpeed);
                allowWallJump = false;
            }
        }
    }
    /// <summary>
    /// Checks for collisions with the ground to ell if player is 'grounded'
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Added ground check sphere
    /// 
    void FixedUpdate()
    {
        //create a sphere that checks if we are on ground
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
    }
    //Debug function to test OnGround check
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
    void OnCollisionEnter2D(Collision2D collide)
    {
        if (collide.gameObject.tag == "Wall")
        {
            allowWallJump = true;
        }
    }
}
