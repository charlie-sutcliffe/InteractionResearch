using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boomerangSpin : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Rotation speed in degrees per second
    public float requiredSpeed = 5f; // Speed threshold to start rotating

    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the object's speed is above the threshold
        if (rb.velocity.magnitude >= requiredSpeed)
        {
            // Rotate the object
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}
