using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Manages the pulling and releasing of the bowstring, supporting multiple interaction methods.
/// </summary>
public class DrawInteraction : XRBaseInteractable
{ 
    public static event Action<float> PullActionReleased;

    [Header("Bowstring Settings")]
    public Transform start, end; // Starting and ending points of bowstring
    public GameObject notch; // Notch where the arrow is nocked

    [Header("Pull Settings")]
    [Tooltip("Maximum pull distance as a normalized value (0 to 1).")]
    public float maxPull = 1.0f;
    public float PullAmount { get; private set; } = 0.0f;

    // SFX
    [Header("Audio Settings")]
    [Tooltip("AudioSource for bow drawing sound.")]
    private AudioSource bowDrawSFX;
    private bool isPlayingSound = false;

    // VFX
    private LineRenderer _lineRenderer;
    private IXRSelectInteractor pullingInteractor = null;

    protected override void Awake() 
    {
        base.Awake();
        _lineRenderer = GetComponent<LineRenderer>();

        bowDrawSFX = GetComponent<AudioSource>();
    }
    
    /// <summary>
    /// Handles external release triggers.
    /// </summary>
    /// <param name="maxPull">Maximum pull distance.</param>
    private void OnPullActionReleased(float maxPull)
    {
        PullAmount = maxPull;
        Release();
    }

    /// <summary>
    /// Public method to allow external scripts to set pull and release the bow.
    /// </summary>
    /// <param name="maxPull">Normalized pull amount (0 to 1).</param>
    public void ExternalRelease(float maxPull)
    {
        PullAmount = maxPull;
        Release();
    }

    /// <summary>
    /// Releases the bowstring, triggering the arrow's flight.
    /// </summary>
    /// <param name="PullAmount">Normalized pull amount (0 to 1).</param>
    private void Release()
    {
        PullActionReleased?.Invoke(PullAmount);
        pullingInteractor = null;
        PullAmount = 0f;
        notch.transform.localPosition = new Vector3(notch.transform.localPosition.x, notch.transform.localPosition.y, 0f);
        UpdateString();
    }

    /// <summary>
    /// Sets the current interactor that is pulling the bowstring.
    /// </summary>
    /// <param name="args">SelectEnterEventArgs containing interactor information.</param>
    public void SetPullInteractor(SelectEnterEventArgs args)
    {
        pullingInteractor = args.interactorObject;
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected)
            {
                Vector3 pullPosition = pullingInteractor.transform.position;
                PullAmount = CalculatePull(pullPosition);
                UpdateString();

                HapticFeedback();
            }
        }
    }

    /// <summary>
    /// Calculates the normalized pull amount based on interactor's position.
    /// </summary>
    /// <param name="pullPosition">Current position of the pulling interactor.</param>
    /// <returns>Normalized pull amount (0 to 1).</returns>
    private float CalculatePull(Vector3 pullPosition)
    {
        Vector3 pullDirection = pullPosition - start.position;
        Vector3 maxPullDirection = end.position - start.position;
        float maxLength = maxPullDirection.magnitude;
        
        maxPullDirection.Normalize();
        float pullLength = Vector3.Dot(pullDirection, maxPullDirection) / maxLength;
        return Mathf.Clamp(pullLength, 0, 1);
    }

    /// <summary>
    /// Updates the visual representation of the bowstring.
    /// </summary>    
    private void UpdateString()
    {
        Vector3 stringPosition = Vector3.forward * Mathf.Lerp(start.transform.localPosition.z, end.transform.localPosition.z, PullAmount);
        notch.transform.localPosition = new Vector3(notch.transform.localPosition.x, notch.transform.localPosition.y, stringPosition.z + 0.2f);
        _lineRenderer.SetPosition(1, stringPosition);

        
    }

    /// <summary>
    /// Provides haptic and audio feedback based on pull amount.
    /// </summary>    
    private void HapticFeedback()
    {
        // SFX
        // FIX sound only playing when string is released
        if (PullAmount > 0.5f && !isPlayingSound)
        {
            PlayDrawSound();
            isPlayingSound = true;
        }
        else if (PullAmount < 0.5f && isPlayingSound)
        {
            bowDrawSFX.Stop();
            isPlayingSound = false;
        }
        // Haptic feedback implementation
        if (pullingInteractor is XRBaseControllerInteractor controllerInteractor)
        {
            controllerInteractor.SendHapticImpulse(PullAmount, 0.1f);
        }
    }

    /// <summary>
    /// Plays the bow drawing sound.
    /// </summary>
    private void PlayDrawSound()
    {
        bowDrawSFX.Play();
    }
}
