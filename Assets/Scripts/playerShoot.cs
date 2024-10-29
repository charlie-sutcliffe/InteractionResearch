using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerShoot : MonoBehaviour
{
    
    [SerializeField] public GameObject BulletTemplate;
    [SerializeField] public float shootPower = 100f;

    public InputActionReference trigger;
    // Start is called before the first frame update
    public AudioClip gunShotSFX;
    void Start()
    {
        trigger.action.performed += Shoot;
    }

    // Update is called once per frame
    void Shoot(InputAction.CallbackContext _)
    {
        GameObject newBullet = Instantiate(BulletTemplate, transform.position, transform.rotation);
        newBullet.GetComponent<Rigidbody>().AddForce(transform.forward*shootPower);
        GetComponent<AudioSource>().PlayOneShot(gunShotSFX);
        //newBullet.GetComponent<
    }
}
