using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfArrow : MonoBehaviour {

    public float arrowLifeDelay;

	// Use this for initialization
	void Start () {
        StartCoroutine(ArrowLife(arrowLifeDelay));
	}

    private IEnumerator ArrowLife(float delay) {
        //Debug.Log("arrow is alive");
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        //Debug.Log("arrow is dead");
    }
}
