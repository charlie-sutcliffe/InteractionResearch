using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 50f;
    public Transform tip;
    
    private Rigidbody rigidBody;
    private bool inAir = false;
    private Vector3 lastPosition = Vector3.zero;
    
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        DrawInteraction.PullActionReleased += Release;
        
        Stop();
    }
    
    private void OnDestroy()
    {
        DrawInteraction.PullActionReleased -= Release;
    }
    
    private void Release(float value)
    {
        DrawInteraction.PullActionReleased -= Release;
        gameObject.transform.parent = null;
        inAir = true;
        SetPhysics(true);
        
        Vector3 force = transform.forward * value * speed;
        rigidBody.AddForce(force, ForceMode.Impulse);
        
        StartCoroutine(RotateWithVelocity());
        
        lastPosition = tip.position;
    }

    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (inAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(rigidBody.velocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }

    void FixedUpdate() {
        if (inAir) {
            CheckCollision();
            lastPosition = tip.position;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Linecast(lastPosition, tip.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject.layer != 8)
            {
                if (hitInfo.transform.TryGetComponent(out Rigidbody body))
                {
                    rigidBody.interpolation = RigidbodyInterpolation.None;
                    transform.parent = hitInfo.transform;
                    body.AddForce(rigidBody.velocity, ForceMode.Impulse);
                }
                Stop();
            }
        }
    }

    private void Stop()
    {
        inAir = false;
        SetPhysics(false);
    }

    private void SetPhysics(bool usePhysics)
    {
        rigidBody.isKinematic = !usePhysics;
        rigidBody.useGravity = usePhysics;
    }
}
