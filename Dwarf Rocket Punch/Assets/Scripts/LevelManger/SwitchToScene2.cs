using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This script sets a boolean to true if the character has reached the end of the level
/// and the sceneController should switch scenes
/// </summary>
/// 
/// Variables
/// 
/// switchScene         Boolean that determines if the scene can be switched
/// transitionToLevel   Level to transition to
/// 
/// Author: Eric Stratechuk     (ES)
/// 

public class SwitchToScene2 : MonoBehaviour {
    public bool switchScene = false;
    public String transitionToLevel;

    /// <summary>
    /// Initialize switchScene to false
    /// </summary>
    /// 
    /// 2018-12-14  ES
    /// 
    void Start () {
        switchScene = false;

    }
	
    /// <summary>
    /// Determines if the player has collided with the end zone and sets switchScene
    /// to true if it has
    /// </summary>
    /// <param name="collision">A collider</param>
    /// 
    /// 2018-12-14 ES
    /// 
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            Debug.Log("Entering New Scene");
            switchScene = true;       
        }
    }

}
