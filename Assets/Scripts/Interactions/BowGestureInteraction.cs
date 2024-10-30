using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles bow firing via hand gestures or body movements.
/// </summary>
public enum Handedness
{
    Left,
    Right
}

public class BowGestureInteraction : MonoBehaviour
{
    [Header("Bow Interaction")]
    [Tooltip("Reference to the DrawInteraction script.")]
    public DrawInteraction drawInteraction;

    [Header("Hand Tracking Settings")]
    [Tooltip("Reference to the HandTrackingManager script.")]
    public HandTrackingManager handTrackingManager;

    [Header("Gesture Settings")]
    [Tooltip("Threshold distance to detect a pull gesture.")]
    public float pullThreshold = 0.3f;

    [Tooltip("Maximum pull strength based on gesture.")]
    public float maxGesturePull = 1.0f;

    [Header("Pull Calculation")]
    [Tooltip("Offset for pull calculation.")]
    public Vector3 pullOffset = new Vector3(0, 0, -0.1f);

    private void Update()
    {
        DetectAndFireGesture();
    }

    /// <summary>
    /// Detects pull gestures and fires the bow accordingly.
    /// </summary>
    private void DetectAndFireGesture()
    {
        if (handTrackingManager.IsHandDetected(Handedness.Left) && handTrackingManager.IsHandDetected(Handedness.Right))
        {
            Vector3 leftHandPos = handTrackingManager.GetHandPosition(Handedness.Left);
            Vector3 rightHandPos = handTrackingManager.GetHandPosition(Handedness.Right);

            // Calculate the distance between hands
            float distance = Vector3.Distance(leftHandPos, rightHandPos);

            // Determine if a pull gesture is performed
            if (distance > pullThreshold)
            {
                // Normalize pull strength
                float pullAmount = Mathf.Clamp01((distance - pullThreshold) / (pullThreshold));

                // Optionally, adjust based on specific gesture criteria
                // e.g., direction, speed, etc.

                // Fire the bow with the calculated pull amount
                if (drawInteraction != null)
                {
                    drawInteraction.ExternalRelease(pullAmount * maxGesturePull);
                }
            }
        }
    }
}
