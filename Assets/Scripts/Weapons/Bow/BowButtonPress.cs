using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class BowButtonPress : MonoBehaviour
{
    public InputActionReference fireAction; // Assign in Inspector
    public GameObject arrowPrefab;
    public GameObject arrowSpawnPoint;
    public float arrowForce = 20f;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        fireAction.action.Enable();
        fireAction.action.performed += FireArrow;
    }

    private void OnDisable()
    {
        fireAction.action.performed -= FireArrow;
        fireAction.action.Disable();
    }

    private void FireArrow(InputAction.CallbackContext context)
    {
        // Get the arrow prefab and spawn point
        if (arrowPrefab == null || arrowSpawnPoint == null)
        {
            Debug.LogError("Arrow prefab or spawn point not assigned in BowButtonPress script.");
            return;
        }
        // Instantiate the arrow at the spawn point
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.transform.position, arrowSpawnPoint.transform.rotation);
        // Get the arrow's Rigidbody component and apply force to the arrow
        arrow.GetComponent<Rigidbody>().AddForce(arrow.transform.forward * arrowForce, ForceMode.Force);

        // Optional: Add haptic feedback
        foreach (var interactable in grabInteractable.interactorsSelecting)
        {
            if (interactable is XRBaseControllerInteractor controllerInteractor)
            {
                controllerInteractor.xrController.SendHapticImpulse(0.5f, 0.1f);
            }
        }
    }
}
