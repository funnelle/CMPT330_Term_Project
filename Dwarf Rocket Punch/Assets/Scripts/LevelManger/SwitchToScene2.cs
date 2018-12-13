using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwitchToScene2 : SceneController {

    //public SceneController sceneController;
    public bool switchScene = false;

    // Use this for initialization
    void Start () {
        switchScene = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Entering New Scene");
            switchScene = true;
            //GetComponent<SceneController>().FadeAndLoadScene("TestLevel2");        
        }
    }

}
