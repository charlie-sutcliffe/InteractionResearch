using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

/// <summary>
/// Handles bow firing via button presses.
/// </summary>
public class BowButtonPress : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("Input action for firing the bow.")]
    public InputActionProperty fireAction; // Assign in Inspector

    [Header("Bow Interaction")]
    [Tooltip("Reference to the DrawInteraction script.")]
    public DrawInteraction drawInteraction;

    [Header("Pull Settings")]
    [Tooltip("Default pull amount when firing via button press.")]
    [Range(0.0f, 1.0f)]
    public float defaultPullAmount = 3.0f;
    private void OnEnable()
    {
        fireAction.action.Enable();
        fireAction.action.performed += OnFireButtonPressed;
    }

    private void OnDisable()
    {
        fireAction.action.performed -= OnFireButtonPressed;
        fireAction.action.Disable();
    }

    /// <summary>
    /// Fires the bow when the fire action is performed.
    /// </summary>
    /// <param name="context">Input action context.</param>
    private void OnFireButtonPressed(InputAction.CallbackContext context)
    {
        // Check that the bow object is being held
        if (drawInteraction.isSelected)
        {
            // Release the bow with the default pull amount
            drawInteraction.ExternalRelease(defaultPullAmount);
        }

        // Optionally, add haptic feedback or other effects
    }
}