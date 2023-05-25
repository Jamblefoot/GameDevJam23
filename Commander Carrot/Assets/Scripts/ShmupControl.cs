using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShmupControl : MonoBehaviour
{
    public Transform cameraAnchor;
    public Transform landingPoint;
    public float width = 20;
    public float length = 10;

    public void MoveToGround(Transform tran)
    {
        
        PlayerControl pc = tran.GetComponent<PlayerControl>();
        if (pc != null)
        {
            pc.SetMoveStyle(MoveStyle.TopFree, -transform.forward);
        }
        FollowCamera cam = FindObjectOfType<FollowCamera>();
        if (cam != null)
        {
            cam.anchor = null;
            cam.MoveToTop(true, Vector3.up);
        }
        ShipDrive sd = tran.GetComponent<ShipDrive>();
        if (sd != null)
        {
            sd.shmupControl = null;
        }
        StartCoroutine(MoveToGroundCo(tran));
    }
    IEnumerator MoveToGroundCo(Transform tran)
    {
        while(tran.position != landingPoint.position)
        {
            tran.position = Vector3.MoveTowards(tran.position, landingPoint.position, Time.deltaTime * 50f);
            tran.rotation = landingPoint.transform.rotation;
            yield return new WaitForFixedUpdate();
        }
        ShipDrive sd = tran.GetComponent<ShipDrive>();
        if (sd != null)
        {
            sd.rigid.useGravity = true;
            sd.rigid.isKinematic = false;
        }
        else
        {
            //just send the player in a t-pose, aiming gun
        }

        //TODO transition to shmup rig
        // camera targetpos = shmuprig camera anchor
        //THIS IS JUST FOR TESTING, PROBABLY NEED SOME REFERENCE TO THE PLAYER DRIVING THE SHIP


        Vector3 shift = new Vector3(0, 0, transform.position.z * -2);
        transform.position = transform.position + shift;
        
    }

}
