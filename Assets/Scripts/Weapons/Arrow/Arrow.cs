using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 30f;
    public Transform tip;
    
    private Rigidbody rigidBody;
    private bool inAir = false;
    private Vector3 lastPosition = Vector3.zero;

    // SFX
    private AudioSource arrowShootSFX;

    // Child of Arrow - Arrow Tip's Audio Source
    public GameObject arrowTipAudioSource;
    private AudioSource arrowTipAudioSourceScript;
    

    // VFX
    private ParticleSystem arrowParticleSystem;
    private TrailRenderer trailRenderer;
    
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        // SFX
        arrowShootSFX = GetComponent<AudioSource>();

        // Child of Arrow - Arrow Tip's Audio Source
        arrowTipAudioSourceScript = arrowTipAudioSource.GetComponent<AudioSource>();

        // VFX
        arrowParticleSystem = GetComponentInChildren<ParticleSystem>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

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
        
        Vector3 force = speed * value * transform.forward;
        rigidBody.AddForce(force, ForceMode.Impulse);
        
        StartCoroutine(RotateWithVelocity());
        
        lastPosition = tip.position;

        // SFX
        PlayArrowShoot();

        // VFX
        arrowParticleSystem.Play();
        trailRenderer.emitting = true;
    }

    // Arrow Flight Rotation
    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (inAir)
        {
            Vector3 direction = rigidBody.velocity;
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);
            }
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

                    // Child of Arrow - Arrow Tip's Audio Source
                    arrowTipAudioSourceScript.Stop();
                    arrowTipAudioSourceScript.Play();
                }
                Stop();
            }
        }
    }

    private void Stop()
    {
        inAir = false;
        SetPhysics(false);

        // VFX
        arrowParticleSystem.Stop();
        trailRenderer.emitting = false;
    }

    private void SetPhysics(bool usePhysics)
    {
        rigidBody.isKinematic = !usePhysics;
        rigidBody.useGravity = usePhysics;
    }

    private void PlayArrowShoot()
    {
        arrowShootSFX.Stop();
        arrowShootSFX.Play();
    }
}
