using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script for handling both the arrow objects life time and for it's interaction with the player, causing the player to respawn.
/// Includes a coroutine from the respawn script for dealing with the player as well, would obviously be preferable to have them both call from elsewhere.
/// </summary>
/// 
/// Field           Summary
/// 
/// *public*
/// arrowLifeDelay  The amount of time to process before the arrow disappears
/// deathCameraTime The amount of time to wait before the camera jumps to the dwarf. 
/// 
/// Author: Eric Walker - OnCollisionEnter2D, Comments
///         Eamonn McCormick - ArrowLife
///         Eric Stratechuk - StopDwarfMovement, slightly edited by Eric Walker for use in the Arrow
public class ElfArrow : MonoBehaviour {

    public float arrowLifeDelay;
    public float deathCameraTime;
    // Use this for initialization
    void Start () {
        StartCoroutine(ArrowLife(arrowLifeDelay));
	}

    /// <summary>
    /// Handles when arrows collide with the player, calling the appropriate coroutine.
    /// </summary>
    /// <param name="collision">The collider we are currently checking</param>
    ///  EW 2018-12-14
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<DwarfController>().allowMovement = false;

            StartCoroutine(StopDwarfMovement(collision, deathCameraTime));
        }
    }

    /// <summary>
    /// Deletes the Arrow GameObject after a set delay. 
    /// </summary>
    /// <param name="delay">The amount of time we wait before destroying it.</param>
    /// <returns>The wait for seconds in game before calling the destroy.</returns>
    /// EPM ~At some point in December~
    private IEnumerator ArrowLife(float delay) {
        //Debug.Log("arrow is alive");
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        //Debug.Log("arrow is dead");
    }

    /// <summary>
    /// Resets the dwarf's position, stops his movement vector and forces the camera to wait for a few seconds before following the dwarf to his new position. 
    /// </summary>
    /// <param name="collision">The player collision.</param>
    /// <param name="deathCameraTime">The amount of delay before the camera follows the dwarf.</param>
    /// <returns>The wait for seconds in game before finishing the coroutine.</returns>
    /// ES
    /// EW 2018-12-14
    public IEnumerator StopDwarfMovement(Collision2D collision, float deathCameraTime)
    {
        //yield return new WaitForSeconds(deathCameraTime);
        collision.gameObject.transform.position = GameObject.FindWithTag("ReSpawnArea").transform.position;
        collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(deathCameraTime);
        collision.gameObject.GetComponent<DwarfController>().allowMovement = true;

    }
}
