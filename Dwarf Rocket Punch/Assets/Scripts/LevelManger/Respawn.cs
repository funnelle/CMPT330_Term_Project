using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the player contacts these areas, respawns them. "Hazard areas."
/// </summary>
/// Field       Summary
/// 
/// deathCameraTime  The amount of time we wait for the camera to find the dwarf again.
/// 
/// Author: Eric Stratechuk - Coding
///         Eric Walker - Commenting
public class Respawn : MonoBehaviour {
    public float deathCameraTime;
    
    /// <summary>
    /// When the player enters this collider, respawn them. 
    /// </summary>
    /// <param name="collision">The collision we are detecting.</param>
    /// ES 
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

    /// <summary>
    /// Respawn the player by resetting the players transform, and make the camera wait for a moment.
    /// </summary>
    /// <param name="collision">The player collision.</param>
    /// <param name="deathCameraTime">The amount of time waiting before pulling the camera.</param>
    /// <returns>The time returned in game seconds before finishing the coroutine.</returns>
    /// ES
    public IEnumerator StopDwarfMovement(Collider2D collision, float deathCameraTime)
    {
        //yield return new WaitForSeconds(deathCameraTime);
        collision.gameObject.transform.position = GameObject.FindWithTag("ReSpawnArea").transform.position;
        collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(deathCameraTime);
        collision.gameObject.GetComponent<DwarfController>().allowMovement = true;

    }
}
