using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Camera follows player character with a short lag
/// </summary>
/// 
/// Field                   Description
/// *public*
/// sceneController         GameObject that holds the SceneController object
/// offset                  Vector3 value of camera position
/// HYSTERESIS              Lag time before Camera follows player (lower num is longer lag time)
/// 
/// *private*
/// player                  Transform position of player. Used as Camera target
/// sceneControllerScript   Script that loads Async scenes.
/// playerFound             Bool value that checks if player is present in the Hierarchy
/// activeScene             Name of currently active scene
/// 
/// Author: Evan Funnell    (EVF)
/// 
public class LooseFollow : MonoBehaviour {
    public GameObject sceneController;
    public Vector3 offset;
    public float HYSTERESIS = 2f;

    private Transform player;
    private SceneController sceneControllerScript;
    private bool playerFound = false;
    private string activeScene;

    /// <summary>
    /// Initialize variables
    /// </summary>
    /// 
    /// 2018-10-10  EVF     Initialize Variables
    /// 
    void Start() {
        sceneControllerScript = sceneController.GetComponent<SceneController>();
    }

    /// <summary>
    /// Finds player in Async loaded scene
    /// </summary>
    /// 
    /// 2018-10-11  EVF     Added checks to see if player exists in loaded scene
    /// 
    void Update() {
        activeScene = sceneControllerScript.activeScene;
        if (activeScene != "" && playerFound == false) {
            player = GameObject.Find("Dwarf").transform;
            playerFound = true;
        }
    }

    /// <summary>
    /// Interpolates position of camera with slight delay
    /// </summary>
    /// 
    /// 2018-10-10  EVF     Added lagged camera tracking
    /// 
    void FixedUpdate () {
        if (player != null) {
            Vector3 startingPosition = player.position + offset;
            Vector3 where = Vector3.Lerp(transform.position, startingPosition, HYSTERESIS * Time.deltaTime);
            transform.position = where;
        }
	}
}
