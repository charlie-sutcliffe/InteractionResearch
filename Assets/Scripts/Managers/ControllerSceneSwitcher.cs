using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles scene switching and restarting based on controller inputs.
/// </summary>
public class ControllerSceneSwitcher : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("List of scene names to switch between.")]
    public List<string> sceneNames;

    [Tooltip("Current scene index.")]
    private int currentSceneIndex = 0;

    [Header("Input Settings")]
    [Tooltip("Input action for Grip button.")]
    public InputActionProperty gripAction;

    [Tooltip("Input action for Trigger button.")]
    public InputActionProperty triggerAction;

    [Tooltip("Input action for Primary Button (e.g., X/A).")]
    public InputActionProperty primaryButtonAction;

    [Tooltip("Input action for Secondary Button (e.g., Y/B).")]
    public InputActionProperty secondaryButtonAction;

    // [Header("Feedback Settings")]
    // [Tooltip("AudioSource for feedback sounds.")]
    // public AudioSource feedbackAudioSource;

    // [Tooltip("AudioClip for scene switch.")]
    // public AudioClip switchSceneClip;

    // [Tooltip("AudioClip for scene restart.")]
    // public AudioClip restartSceneClip;

    // Flags to track button states
    private bool isGripPressed = false;

    void OnEnable()
    {
        // Enable input actions
        gripAction.action.Enable();
        triggerAction.action.Enable();
        primaryButtonAction.action.Enable();
        secondaryButtonAction.action.Enable();
    }

    void OnDisable()
    {
        // Disable input actions
        gripAction.action.Disable();
        triggerAction.action.Disable();
        primaryButtonAction.action.Disable();
        secondaryButtonAction.action.Disable();
    }

    void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// Handles controller inputs to switch or restart scenes.
    /// </summary>
    private void HandleInput()
    {
        // Update Grip state
        isGripPressed = gripAction.action.ReadValue<float>() > 0.5f;

        if (isGripPressed)
        {
            // Check for Trigger + Grip to switch to next scene
            if (triggerAction.action.WasPressedThisFrame())
            {
                SwitchToNextScene();
            }

            // Check for Primary Button + Grip to switch to previous scene
            if (primaryButtonAction.action.WasPressedThisFrame())
            {
                SwitchToPreviousScene();
            }

            // Check for Secondary Button + Grip to restart current scene
            if (secondaryButtonAction.action.WasPressedThisFrame())
            {
                RestartCurrentScene();
            }
        }
    }

    /// <summary>
    /// Switches to the next scene in the list.
    /// </summary>
    private void SwitchToNextScene()
    {
        if (sceneNames.Count == 0)
        {
            Debug.LogWarning("No scenes are assigned in ControllerSceneSwitcher.");
            return;
        }

        currentSceneIndex = (currentSceneIndex + 1) % sceneNames.Count;
        LoadSceneByName(sceneNames[currentSceneIndex]);

        // Play feedback sound
        // PlayFeedbackSound(switchSceneClip);
    }

    /// <summary>
    /// Switches to the previous scene in the list.
    /// </summary>
    private void SwitchToPreviousScene()
    {
        if (sceneNames.Count == 0)
        {
            Debug.LogWarning("No scenes are assigned in ControllerSceneSwitcher.");
            return;
        }

        currentSceneIndex--;
        if (currentSceneIndex < 0)
        {
            currentSceneIndex = sceneNames.Count - 1;
        }
        LoadSceneByName(sceneNames[currentSceneIndex]);

        // Play feedback sound
        // PlayFeedbackSound(switchSceneClip);
    }

    /// <summary>
    /// Reloads the current active scene.
    /// </summary>
    private void RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadSceneByName(currentSceneName);

        // Play feedback sound
        // PlayFeedbackSound(restartSceneClip);
    }

    /// <summary>
    /// Loads a scene asynchronously by its name.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    private void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty in ControllerSceneSwitcher.");
            return;
        }

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Coroutine to load a scene asynchronously.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Optional: Implement a loading screen or progress indicator here

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Plays a feedback sound if an AudioSource and AudioClip are assigned.
    /// </summary>
    /// <param name="clip">AudioClip to play.</param>
    private void PlayFeedbackSound(AudioClip clip)
    {
        // if (feedbackAudioSource != null && clip != null)
        // {
        //     feedbackAudioSource.PlayOneShot(clip);
        // }
    }
}
