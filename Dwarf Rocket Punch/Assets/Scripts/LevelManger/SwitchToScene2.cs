using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwitchToScene2 : MonoBehaviour {

    public bool switchScene = false;
    public String transitionToLevel;

    // Use this for initialization
    void Start () {
        switchScene = false;

    }
	
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Entering New Scene");
            switchScene = true;
        }
    }

}
