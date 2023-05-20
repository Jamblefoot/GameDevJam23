using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentTrigger : MonoBehaviour
{
    public bool alignToZ;
    void OnTriggerEnter(Collider col)
    {
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if(rb != null && !rb.isKinematic)
        {
            if(alignToZ)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                col.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, transform.position.z);
                
            }
            else 
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }

        }
    }
}
