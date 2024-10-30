using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class DistanceGrabber : MonoBehaviour
{
    private XRRayInteractor rayInteractor;
    private XRGrabInteractable currentInteractable;

    [SerializeField]
    private InputActionReference selectAction;

    [Header("Visual Feedback")]
    [Tooltip("Prefab for the grab indicator.")]
    public GameObject grabIndicatorPrefab;
    private GameObject grabIndicator;

    [Header("Grab Settings")]
    [Tooltip("Maximum distance for grabbing objects.")]
    public float maxGrabDistance = 10f;

    [Tooltip("Layer mask to specify which objects can be grabbed.")]
    public InteractionLayerMask interactableLayerMask;

    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        rayInteractor.maxRaycastDistance = maxGrabDistance;
        rayInteractor.interactionLayers = interactableLayerMask;
    }

    private void Update()
    {
        HandleGrab();
    }

    /// <summary>
    /// Handles the grab logic based on ray intersection and input.
    /// </summary>
    private void HandleGrab()
    {
        // Perform a raycast to detect interactable objects
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo))
        {
            XRGrabInteractable interactable = hitInfo.collider.GetComponent<XRGrabInteractable>();

            if (interactable != null)
            {
                // Show grab indicator
                if (grabIndicator == null)
                {
                    grabIndicator = Instantiate(grabIndicatorPrefab, hitInfo.point, Quaternion.identity);
                    grabIndicator.transform.parent = hitInfo.transform;
                }
                else
                {
                    grabIndicator.transform.position = hitInfo.point;
                }

                // Check for grab input (e.g., trigger button pressed)
                if (selectAction.action.WasPressedThisFrame())
                {
                    rayInteractor.StartManualInteraction(interactable as IXRSelectInteractable);
                    currentInteractable = interactable;
                }
            }
            else
            {
                RemoveGrabIndicator();
            }
        }
        else
        {
            RemoveGrabIndicator();
        }

        // Handle release
        if (currentInteractable != null && selectAction.action.WasPerformedThisFrame())
        {
            rayInteractor.EndManualInteraction();
            currentInteractable = null;
            RemoveGrabIndicator();
        }
    }

    /// <summary>
    /// Removes the grab indicator from the scene.
    /// </summary>
    private void RemoveGrabIndicator()
    {
        if (grabIndicator != null)
        {
            Destroy(grabIndicator);
            grabIndicator = null;
        }
    }
}
