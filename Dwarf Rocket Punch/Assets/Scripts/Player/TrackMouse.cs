using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks mouse location in world and points Dwarf's arms at the mouse
/// </summary>
/// 
/// Field           Description
/// mouseLocation   Vector3 position of the mouse in the game
/// armDirection    Vector2 position that points the arm at mouseLocation
/// 
/// Author: Evan Funnell (EVF)
/// 
public class TrackMouse : MonoBehaviour {
    private Vector3 mouseLocation;
    public Vector2 armDirection;
    public static int directionModifier = 1;

	/// <summary>
    /// Gets mouse position, coverts to world space and points arm at mouse position
    /// </summary>
    /// 
    /// 2018-10-12  EVF     Added basic functionality
    /// 
	void Update () {
        mouseLocation = Input.mousePosition;
        mouseLocation = Camera.main.ScreenToWorldPoint(mouseLocation);

        armDirection = new Vector2(mouseLocation.x - transform.position.x, mouseLocation.y - transform.position.y)*directionModifier;

        transform.right = armDirection;
	}
}
