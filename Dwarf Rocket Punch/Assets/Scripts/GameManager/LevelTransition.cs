using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour {
    public GameObject sceneController;
    //public GameObject EndOfLevelZone;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (GameObject.Find("EndOfLevelZone").GetComponent<SwitchToScene2>().switchScene)
        {
            Debug.Log("In the game manager");

            sceneController.GetComponent<SceneController>().FadeAndLoadScene("TestLevel2");
        }
		
	}
}
