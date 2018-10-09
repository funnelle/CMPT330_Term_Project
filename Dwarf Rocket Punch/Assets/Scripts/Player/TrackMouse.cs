using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMouse : MonoBehaviour {
    private Vector3 mouseLocation;
    private Vector2 armDirection;

	// Update is called once per frame
	void Update () {
        mouseLocation = Input.mousePosition;
        mouseLocation = Camera.main.ScreenToWorldPoint(mouseLocation);

        armDirection = new Vector2(mouseLocation.x - transform.position.x, mouseLocation.y - transform.position.y);

        transform.right = armDirection;
	}
}
