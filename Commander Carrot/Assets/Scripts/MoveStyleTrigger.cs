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
            pc.SetMoveStyle(moveStyle, transform.right);//axis);
        }

        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            RigidbodyControl rbc = col.GetComponent<RigidbodyControl>();

            switch (axis)
            {
                /*case AlignmentAxis.X:
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
                    break;*/
                case AlignmentAxis.X:
                case AlignmentAxis.Y:
                case AlignmentAxis.Z:
                    Plane p = new Plane(transform.right, transform.position);
                    col.transform.position = p.ClosestPointOnPlane(col.transform.position);
                    if(rbc != null)
                    {
                        rbc.enabled = true;
                        rbc.constraintPlane = p;
                    }
                    break;
                case AlignmentAxis.None:
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                    if(rbc != null)
                        rbc.enabled = false;
                    break;
            }

        }
    }
}
