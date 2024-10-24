using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BowDraw : XRGrabInteractable
{
    // Reference to the bowstring transform
    public Transform bowstring;
    // Arrow prefab to instantiate
    public GameObject arrowPrefab;
    // Pull strength variables
    private float pullAmount;
    private XRBaseInteractor pullingInteractor;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // Start pulling the bowstring
        pullingInteractor = args.interactorObject as XRBaseInteractor;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // Release the arrow
        FireArrow();
        pullingInteractor = null;
        pullAmount = 0f;
    }

    void Update()
    {
        if (pullingInteractor != null)
        {
            // Calculate pull amount based on hand position
            pullAmount = CalculatePull();
            // Update bowstring position
            UpdateBowstringPosition();
        }
    }

    private float CalculatePull()
    {
        // Implement calculation based on hand distance
        return Mathf.Clamp01(Vector3.Distance(pullingInteractor.transform.position, bowstring.position));
    }

    private void UpdateBowstringPosition()
    {
        // Update bowstring visual based on pull amount
    }

    private void FireArrow()
    {
        // Instantiate arrow and apply force based on pull amount
        GameObject arrow = Instantiate(arrowPrefab, bowstring.position, bowstring.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.velocity = bowstring.forward * pullAmount * 20f; // Adjust force multiplier as needed
    }
}
