using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseFollow : MonoBehaviour {
    //private const float HYSTERESIS = 20f;

    private Transform player;

    public Vector3 offset;
    public float HYSTERESIS = 10f;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Dwarf").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 startingPosition = player.position + offset;
        Vector3 where = Vector3.Lerp(transform.position, startingPosition, HYSTERESIS*Time.deltaTime);
        transform.position = where;
	}
}
