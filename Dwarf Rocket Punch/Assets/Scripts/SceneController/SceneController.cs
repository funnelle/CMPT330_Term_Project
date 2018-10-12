using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Facilitates the loading and unloading of scenes through a persistant scene
/// </summary>
/// 
/// Field               Description
/// *public*
/// faderCanvasGroup    CanvasGroup that stores details about the fader image used between scene changes
/// fadeDuration        Time to fade either in or out
/// startingSceneName   Name of Scene to load at game start
/// activeScene         currently active Async scene within persistent scene
/// 
/// *private*
/// isFading            Boolean true if fade image is changing, false if not
/// 
/// Author: Evan Funnell    (EVF)
/// 
public class SceneController : MonoBehaviour {
    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 1f;
    public string startingSceneName = "MechanicsTestMap";
    public string activeScene;

    private bool isFading;

	/// <summary>
    /// Initializes instance variables, loads starting scene and fades out image
    /// </summary>
    /// 
    /// 2018-10-11  EVF     Initial State
    /// 
    private IEnumerator Start () {
        faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName));

        StartCoroutine(Fade(0f));
	}

    /// <summary>
    /// Called to change scene to given scene
    /// </summary>
    /// <param name="sceneName">Scene name we are changing to</param>
    /// 
    /// 2018-10-11  EVF     Initial State
    /// 
    public void FadeAndLoadScene(string sceneName) {
        if (!isFading) {
            StartCoroutine(FadeAndSwitchScenes(sceneName));
        }
    }

    /// <summary>
    /// Fades image in, unloads the previous scene, calls function to load new scene,
    /// Fades image out 
    /// </summary>
    /// <param name="sceneName">Scene name to load in</param>
    /// 
    /// 2018-10-11  EVF     Initial State
    /// 
    private IEnumerator FadeAndSwitchScenes(string sceneName) {
        yield return StartCoroutine(Fade(1f));

        /*
        if (BeforeSceneUnload != null) {
            BeforeSceneUnload();
        }
        */

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        /*
        if (AfterSceneLoad != null) {
            AfterSceneLoad();
        }
        */
        yield return StartCoroutine(Fade(0f));
    }

    /// <summary>
    /// Loads the scene and set active.
    /// </summary>
    /// <param name="sceneName">Scene name of scene we are changing to</param>
    /// 
    /// 2018-10-11  EVF     Added scene load and set active code
    /// 
    private IEnumerator LoadSceneAndSetActive(string sceneName) {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newlyLoadedScene);
        activeScene = newlyLoadedScene.name;

    }

    /// <summary>
    /// Fade the image to the specified finalAlpha.
    /// </summary>
    /// <param name="finalAlpha">Final alpha.</param>
    /// 
    /// 2018-10-11  EVF     Fade image to given value
    /// 
    private IEnumerator Fade(float finalAlpha) {
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha)) {
            faderCanvasGroup.alpha =
                Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }
}
