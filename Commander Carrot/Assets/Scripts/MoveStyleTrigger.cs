using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveStyleTrigger : MonoBehaviour
{
    public MoveStyle moveStyle;
    //public AlignmentAxis axis = AlignmentAxis.None;

    public ShmupControl targetRig;

    public List<Collider> movingCols = new List<Collider>();
    
    void OnTriggerEnter(Collider col)
    {
        PlayerControl pc = col.GetComponent<PlayerControl>();
        if(pc != null && moveStyle != MoveStyle.TopShmup)
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

                    if (!movingCols.Contains(col) && (col.GetComponent<ShipDrive>() || col.GetComponent<PlayerControl>()))
                    {
                        movingCols.Add(col);
                        targetRig.MoveToRig(col.transform);
                        //StartCoroutine(MoveToShmup(col));
                    }
                    /*ShipDrive sd = col.GetComponent<ShipDrive>();
                    if(sd != null && !movingCols.Contains(col))
                    {
                        
                    }
                    else
                    {
                        //just send the player in a t-pose, aiming gun
                    }*/


                    
                    break;
            }

        }
    }

    void OnTriggerExit(Collider col)
    {
        if(movingCols.Contains(col))
        {
            movingCols.Remove(col);
        }
    }

    /*IEnumerator MoveToShmup(Collider col)
    {
        ShipDrive sd = col.GetComponent<ShipDrive>();
        if(sd != null)
        {
            sd.rigid.useGravity = false;
            sd.rigid.isKinematic = true;
            Vector3 targetPos = targetRig.transform.position - targetRig.transform.forward * 7;
            while(sd.transform.position != targetPos)
            {
                
                //sd.transform.position = targetRig.transform.position - targetRig.transform.forward * 7;
                sd.transform.position = Vector3.MoveTowards(sd.transform.position, targetPos, Time.deltaTime * 50f);
                sd.transform.rotation = targetRig.transform.rotation;
                yield return new WaitForFixedUpdate();
            }
            sd.shmupControl = targetRig;
        }
        else
        {
            //just send the player in a t-pose, aiming gun
        }       

        //TODO transition to shmup rig
        // camera targetpos = shmuprig camera anchor
        //THIS IS JUST FOR TESTING, PROBABLY NEED SOME REFERENCE TO THE PLAYER DRIVING THE SHIP

        movingCols.Remove(col);


        FollowCamera cam = FindObjectOfType<FollowCamera>();
        cam.MoveToAnchor(targetRig.cameraAnchor);

        PlayerControl pc = col.GetComponent<PlayerControl>();
        if (pc != null)
        {
            pc.SetMoveStyle(moveStyle, -transform.forward);//axis);
        }

        //Move targetRig, camera, and col.transform to other side of the map
        Vector3 shift = new Vector3(0, 0, targetRig.transform.position.z * -2);
        col.transform.position = col.transform.position + shift;
        cam.transform.position = cam.transform.position + shift;
        targetRig.transform.position = targetRig.transform.position + shift;


        targetRig.GetComponentInChildren<EnemySpawner>()?.StartSpawning(20);
    }*/
}
