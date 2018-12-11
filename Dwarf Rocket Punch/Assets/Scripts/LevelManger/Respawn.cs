using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {
    public float deathCameraTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<DwarfController>().allowMovement = false;

            //collision.gameObject.transform.position = GameObject.FindWithTag("ReSpawnArea").transform.position;
            //collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            StartCoroutine(StopDwarfMovement(collision, deathCameraTime));

        }
    }

    public IEnumerator StopDwarfMovement(Collider2D collision, float deathCameraTime)
    {
        //yield return new WaitForSeconds(deathCameraTime);
        collision.gameObject.transform.position = GameObject.FindWithTag("ReSpawnArea").transform.position;
        collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(deathCameraTime);
        collision.gameObject.GetComponent<DwarfController>().allowMovement = true;

    }
}
