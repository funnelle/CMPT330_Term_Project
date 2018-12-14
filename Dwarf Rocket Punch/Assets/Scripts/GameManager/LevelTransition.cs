using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class LevelTransition : MonoBehaviour {
    public GameObject sceneController;
    private SwitchToScene2 swapScript;
    public GameObject swapSceneObject;

    private SceneController sceneControllerScript;
    private string activeScene;

    // Use this for initialization
    void Start () {
        sceneControllerScript = sceneController.GetComponent<SceneController>();


	}
	
	// Update is called once per frame
	void Update () {
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
