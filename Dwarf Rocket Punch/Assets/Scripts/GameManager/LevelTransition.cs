using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script causes the sceneController to switch from level 1 to level 2 upon reaching the completion zone
/// </summary>
/// 
/// Variables
/// 
/// sceneController     Reference to the sceneController
/// 
/// Author: Eric Stratechuk     (ES)
/// 

public class LevelTransition : MonoBehaviour {

    public GameObject sceneController;
	
    /// <summary>
    /// Checks if the player has reached the end of level. If he has, switch to scene 2
    /// </summary>
    /// 
    /// 2018-12-14  ES
    /// 
	void Update () {

        if (GameObject.Find("EndOfLevelZone").GetComponent<SwitchToScene2>().switchScene)
        {
            Debug.Log("In the game manager");

            sceneController.GetComponent<SceneController>().FadeAndLoadScene("TestLevel2");
        }
		
	}
}
