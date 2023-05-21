using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum AlignmentAxis { None, X, Y, Z };
public class AlignmentTrigger : MonoBehaviour
{
    
    public AlignmentAxis axis = AlignmentAxis.Z;
    //public bool alignToZ;
    void OnTriggerEnter(Collider col)
    {
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if(rb != null && !rb.isKinematic)
        {
            switch(axis)
            {
                case AlignmentAxis.X:
                    if (col.GetComponent<PlayerControl>())
                        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
                    else rb.constraints = RigidbodyConstraints.FreezePositionX;
                    col.transform.position = new Vector3(transform.position.x, col.transform.position.y, col.transform.position.z);
                    break;
                case AlignmentAxis.Y:
                    if (col.GetComponent<PlayerControl>())
                        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    else rb.constraints = RigidbodyConstraints.FreezePositionY;
                    col.transform.position = new Vector3(col.transform.position.x, transform.position.y, col.transform.position.z);
                    break;
                case AlignmentAxis.Z:
                    if (col.GetComponent<PlayerControl>())
                        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                    else rb.constraints = RigidbodyConstraints.FreezePositionZ;
                    col.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, transform.position.z);
                    break;
                case AlignmentAxis.None:
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                    break;
            }
            /*if(alignToZ)
            {
                if(col.GetComponent<PlayerControl>())
                    rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                else rb.constraints = RigidbodyConstraints.FreezePositionZ;
                col.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, transform.position.z);
                
            }
            else 
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }*/

        }
    }
}
