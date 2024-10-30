using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

/// <summary>
/// Manages hand tracking data for detecting gestures.
/// </summary>
public class HandTrackingManager : MonoBehaviour
{
    // The hand tracking data
    private XRHandSubsystem leftHandSubsystem;
    private XRHandSubsystem rightHandSubsystem;

    void Start()
    {
        // Initialize and enable hand subsystems
        List<XRHandSubsystemDescriptor> descriptors = new List<XRHandSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors(descriptors);
        foreach (var descriptor in descriptors)
        {
            var subsystem = descriptor.Create();
            if (!subsystem.running)
            {
                subsystem.Start();
                if (subsystem.GetType().Name.Contains("Left"))
                    leftHandSubsystem = subsystem;
                else if (subsystem.GetType().Name.Contains("Right"))
                    rightHandSubsystem = subsystem;
            }
        }
    }

    /// <summary>
    /// Checks if a specific hand is detected and tracked.
    /// </summary>
    /// <param name="handedness">Left or Right hand.</param>
    /// <returns>True if the hand is detected and tracked; otherwise, false.</returns>
    public bool IsHandDetected(Handedness handedness)
    {
        if (handedness == Handedness.Left)
            return leftHandSubsystem != null && leftHandSubsystem.leftHand.isTracked;
        else
            return rightHandSubsystem != null && rightHandSubsystem.rightHand.isTracked;
    }

    /// <summary>
    /// Gets the position of a specific hand.
    /// </summary>
    /// <param name="handedness">Left or Right hand.</param>
    /// <returns>Position of the hand; Vector3.zero if not detected.</returns>
    public Vector3 GetHandPosition(Handedness handedness)
    {
        if (handedness == Handedness.Left && leftHandSubsystem != null)
            return leftHandSubsystem.leftHand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose pose) ? pose.position : Vector3.zero;
        if (handedness == Handedness.Right && rightHandSubsystem != null)
            return rightHandSubsystem.rightHand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose pose) ? pose.position : Vector3.zero;
        return Vector3.zero;
    }
}


