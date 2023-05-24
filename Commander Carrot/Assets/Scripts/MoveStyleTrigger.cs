using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveStyleTrigger : MonoBehaviour
{
    public MoveStyle moveStyle;
    //public AlignmentAxis axis = AlignmentAxis.None;

    public ShmupControl targetRig;
    
    void OnTriggerEnter(Collider col)
    {
        PlayerControl pc = col.GetComponent<PlayerControl>();
        if(pc != null)
        {
            pc.SetMoveStyle(moveStyle, -transform.forward);//axis);
        }

        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            RigidbodyControl rbc = col.GetComponent<RigidbodyControl>();

            switch (moveStyle)
            {
                case MoveStyle.Side:
                    Plane p = new Plane(-transform.forward, transform.position);
                    col.transform.position = p.ClosestPointOnPlane(col.transform.position);
                    if(rbc != null)
                    {
                        rbc.enabled = true;
                        rbc.constraintPlane = p;
                    }
                    break;
                case MoveStyle.TopFree:
                    if(pc != null)
                        rb.constraints = RigidbodyConstraints.FreezeRotation;
                    if(rbc != null)
                        rbc.enabled = false;
                    break;
                case MoveStyle.TopShmup:
                    if(targetRig == null)
                    {
                        Debug.LogError("No Shmup Rig To Transition To");
                        break;
                    }

                    ShipDrive sd = col.GetComponent<ShipDrive>();
                    if(sd != null)
                    {
                        sd.shmupControl = targetRig;
                        col.transform.position = targetRig.transform.position - targetRig.transform.forward * 7;
                        col.transform.rotation = targetRig.transform.rotation;
                        sd.rigid.useGravity = false;
                        sd.rigid.isKinematic = true;

                        //TODO transition to shmup rig
                        // camera targetpos = shmuprig camera anchor
                        //THIS IS JUST FOR TESTING, PROBABLY NEED SOME REFERENCE TO THE PLAYER DRIVING THE SHIP
                        FollowCamera cam = FindObjectOfType<FollowCamera>();
                        cam.MoveToAnchor(targetRig.cameraAnchor);

                        targetRig.GetComponentInChildren<EnemySpawner>()?.StartSpawning(20);
                    }
                    


                    
                    break;
            }

        }
    }
}
