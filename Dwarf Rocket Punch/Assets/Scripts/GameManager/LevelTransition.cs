using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script causes the sceneController to switch from level 1 to level 2 upon reaching the completion zone
/// </summary>
/// 
/// Variables
/// 
/// sceneController         Reference to the sceneController
/// swapScript              Reference to SwitchToScene2 script
/// swapSceneObject         Object used to swap scenes
/// sceneControllerScript   A reference to the SceneContoller
/// activeScene             A string containing the name of the active scene
/// 
/// Author: Eric Stratechuk     (ES)
/// 

public class LevelTransition : MonoBehaviour
{
    public GameObject sceneController;
    private SwitchToScene2 swapScript;
    public GameObject swapSceneObject;

    private SceneController sceneControllerScript;
    private string activeScene;

    /// <summary>
    /// Get the SceneController Component
    /// </summary>
    /// 
    /// 2018-12-14  ES
    /// 
    void Start()
    {
        sceneControllerScript = sceneController.GetComponent<SceneController>();
    }

    /// <summary>
    /// Checks if the player has reached the end of level. If he has, switch to scene 2
    /// </summary>
    /// 
    /// 2018-12-14  ES
    /// 
    void Update()
    {
        if (activeScene != "")
        {
            swapScript = GameObject.Find("EndOfLevelZone").GetComponent<SwitchToScene2>();
            if (swapScript.switchScene)
            {
                Debug.Log("Switching to next scener");

                sceneController.GetComponent<SceneController>().FadeAndLoadScene(swapScript.transitionToLevel);
            }

        }

    }
}