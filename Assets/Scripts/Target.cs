using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Amplitude of the target's movement
    public float movementAmplitude = 1f;

    // Speed of the target's movement
    public float movementSpeed = 1f;

    // Starting position of the target
    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position so your targets don't all jump around
        startPosition = transform.position;
    }

    void Update()
    {
        // Each frame, update the transform.position to make the target move
        // Use Mathf.Sin() to create oscillating movement
        float movementOffset = Mathf.Sin(Time.time * movementSpeed) * movementAmplitude;
        transform.position = startPosition + new Vector3(movementOffset, 0f, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the target collided with a projectile
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Destroy the target
            Destroy(gameObject);

            // Destroy the projectile as well
            Destroy(collision.gameObject);
        }
    }
}