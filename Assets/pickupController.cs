using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class pickupController : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] Transform holdArea;
    private GameObject heldObj;
    private Rigidbody heldObjRB;

    [HeaderAttribute("Physics Parameters")]
    [SerializeField] private float pickupRange = 5.0f;
    [SerializeField] private float pickupForce = 150.0f;

    //Input detection
    [SerializeField] private InputActionReference trigger;

    //Temp object storage
    private GameObject throwObject;
    private Vector3 rotation;
    [SerializeField] float throwForce = 0.3f;

    private bool alreadyDone = false;

    private void Update() 
    {
        if (trigger.action.ReadValue<float>() > 0.5f && !alreadyDone)
        {
            alreadyDone = true;

            if (heldObj == null) 
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position, transform.forward, out hit, pickupRange)) 
                {
                    //Pickup Object
                    PickupObject(hit.transform.gameObject);
                }
            }
            else
            {
                //Drop Object
                //Check if object is throwable
                if (heldObj.tag == "Throwable")
                {
                    ThrowObject();
                }
                else
                {
                    DropObject();
                }
            }
        }
        else if (trigger.action.ReadValue<float>() < 0.2f)
        {
            alreadyDone = false;
        }

        if(heldObj != null)
        {
            //MoveObject
            MoveObject();
        }
    }

    void MoveObject()
    {
        if(Vector3.Distance(heldObj.transform.position, holdArea.position) > .1f)
        {
            Vector3 moveDirection = (holdArea.position - heldObj.transform.position);
            heldObjRB.AddForce(moveDirection * pickupForce);

            //heldObjRB.transform.rotation = gameObject.transform.rotation;
        }
    }

    void PickupObject(GameObject pickObj) 
    {
        if(pickObj.gameObject.tag == "Grabbable" || pickObj.gameObject.tag == "Throwable")
        {
            if(pickObj.GetComponent<Rigidbody>())
            {
                heldObjRB = pickObj.GetComponent<Rigidbody>();
                heldObjRB.useGravity = false;
                heldObjRB.drag = 10;

                heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

                heldObjRB.transform.parent = holdArea;
                heldObj = pickObj;
            }
        }
    }

    void DropObject() 
    {
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObjRB.constraints = RigidbodyConstraints.None;

        heldObjRB.transform.parent = null;
        heldObj = null;
    }

    void ThrowObject()
    {
        throwObject = heldObj;

        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObjRB.constraints = RigidbodyConstraints.None;

        heldObjRB.transform.parent = null;
        heldObj = null;

        rotation = gameObject.transform.forward;

        //Debug.Log(throwObject);

        throwObject.GetComponent<Rigidbody>().velocity = new UnityEngine.Vector3(0, 0, 0);
        throwObject.GetComponent<Rigidbody>().AddForce(rotation * throwForce);
    }
}