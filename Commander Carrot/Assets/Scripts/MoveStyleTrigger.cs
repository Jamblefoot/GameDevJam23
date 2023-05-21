using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveStyleTrigger : MonoBehaviour
{
    public MoveStyle moveStyle;
    public AlignmentAxis axis = AlignmentAxis.None;
    
    void OnTriggerEnter(Collider col)
    {
        PlayerControl pc = col.GetComponent<PlayerControl>();
        if(pc != null)
        {
            pc.SetMoveStyle(moveStyle, axis);
        }

        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            switch (axis)
            {
                case AlignmentAxis.X:
                    if (pc != null)
                        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
                    else rb.constraints = RigidbodyConstraints.FreezePositionX;
                    col.transform.position = new Vector3(transform.position.x, col.transform.position.y, col.transform.position.z);
                    break;
                case AlignmentAxis.Y:
                    if (pc != null)
                        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    else rb.constraints = RigidbodyConstraints.FreezePositionY;
                    col.transform.position = new Vector3(col.transform.position.x, transform.position.y, col.transform.position.z);
                    break;
                case AlignmentAxis.Z:
                    if (pc != null)
                        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                    else rb.constraints = RigidbodyConstraints.FreezePositionZ;
                    col.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, transform.position.z);
                    break;
                case AlignmentAxis.None:
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                    break;
            }

        }
    }
}
