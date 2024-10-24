using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunButton : MonoBehaviour
{
    void OnCollisionEnter(Collision other){
        transform.parent.GetComponent<playerShoot>().enabled = true;
    }
}