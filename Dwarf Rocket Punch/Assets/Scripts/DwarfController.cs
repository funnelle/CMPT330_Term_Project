using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfController : MonoBehaviour {

    public bool allowMovement = true;
    public float maxSpeed = 10f;

    private Rigidbody2D rb2d;
    private float movementSpeed;

	// Use this for initialization
	void Start () {
        rb2d = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (allowMovement) {
            movementSpeed = Input.GetAxis("Horizontal");
            rb2d.velocity = new Vector2(movementSpeed * maxSpeed, 0f);
        }
    }
}
