using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LooseFollow : MonoBehaviour {
    public GameObject sceneController;
    public Vector3 offset;
    public float HYSTERESIS = 2f;

    private Transform player;
    private SceneController sceneControllerScript;
    private bool playerFound = false;
    private string activeScene;

    private void Start() {
        sceneControllerScript = sceneController.GetComponent<SceneController>();
    }

    void Update() {
        activeScene = sceneControllerScript.activeScene;
        if (activeScene != "" && playerFound == false) {
            player = GameObject.Find("Dwarf").transform;
            playerFound = true;
        }
    }

    void FixedUpdate () {
        if (player != null) {
            Vector3 startingPosition = player.position + offset;
            Vector3 where = Vector3.Lerp(transform.position, startingPosition, HYSTERESIS * Time.deltaTime);
            transform.position = where;
        }
	}
}
