using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShmupControl : MonoBehaviour
{
    public Transform cameraAnchor;
    public Transform landingPoint;
    public float width = 20;
    public float length = 10;

    EnemySpawner enemySpawner;

    void Start()
    {
        enemySpawner = GetComponentInChildren<EnemySpawner>();
    }

    public void MoveToRig(Transform tran)
    {
        Debug.Log("MOVE TO RIG CALLED");
        StartCoroutine(MoveToRigCo(tran));
    }
    IEnumerator MoveToRigCo(Transform tran)
    {
        ShipDrive sd = tran.GetComponent<ShipDrive>();
        PlayerControl pc = tran.GetComponent<PlayerControl>();
        if (sd != null)
        {
            sd.rigid.useGravity = false;
            sd.rigid.isKinematic = true;
            Vector3 targetPos = transform.position - transform.forward * 7;
            while (tran.position != targetPos)
            {

                //sd.transform.position = targetRig.transform.position - targetRig.transform.forward * 7;
                tran.position = Vector3.MoveTowards(tran.position, targetPos, Time.deltaTime * 50f);
                tran.rotation = transform.rotation;
                yield return new WaitForFixedUpdate();
            }
            sd.shmupControl = this;
        }
        else if(pc != null)
        {
            pc.rigid.useGravity = false;
            pc.rigid.isKinematic = true;
            Vector3 targetPos = transform.position - transform.forward * 7;
            while (tran.position != targetPos)
            {

                //sd.transform.position = targetRig.transform.position - targetRig.transform.forward * 7;
                tran.position = Vector3.MoveTowards(tran.position, targetPos, Time.deltaTime * 50f);
                tran.rotation = transform.rotation;
                yield return new WaitForFixedUpdate();
            }
            pc.shmupControl = this;
        }
        // camera targetpos = shmuprig camera anchor
        //THIS IS JUST FOR TESTING, PROBABLY NEED SOME REFERENCE TO THE PLAYER DRIVING THE SHIP

        //movingCols.Remove(col);


        FollowCamera cam = FindObjectOfType<FollowCamera>();
        cam.MoveToAnchor(cameraAnchor);

        
        if (pc != null)
        {
            pc.SetMoveStyle(MoveStyle.TopShmup, -transform.forward);//axis);
        }

        //Move targetRig, camera, and col.transform to other side of the map
        Vector3 shift = new Vector3(0, 0, transform.position.z * -2);
        tran.position = tran.position + shift;
        cam.transform.position = cam.transform.position + shift;
        transform.position = transform.position + shift;


        GetComponentInChildren<EnemySpawner>()?.StartSpawning(20);

        FindObjectOfType<LevelManager>()?.SpawnNewLevel();
    }

    public void MoveToGround(Transform tran)
    {

        TutorialControl.singleton.SetTutorialText("", -1);
        
        PlayerControl pc = tran.GetComponent<PlayerControl>();
        if (pc != null)
        {
            pc.SetMoveStyle(MoveStyle.TopFree, -transform.forward);
            pc.shmupControl = null;
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
        while(Vector3.Distance(tran.position,landingPoint.position) > 1f)
        {
            tran.position = Vector3.MoveTowards(tran.position, landingPoint.position, Time.deltaTime * 50f);
            tran.rotation = landingPoint.transform.rotation;
            yield return new WaitForFixedUpdate();
        }
        ShipDrive sd = tran.GetComponent<ShipDrive>();
        PlayerControl pc = tran.GetComponent<PlayerControl>();
        if (sd != null)
        {
            sd.rigid.useGravity = true;
            sd.rigid.isKinematic = false;
        }
        else
        {
            pc.rigid.useGravity = true;
            pc.rigid.isKinematic = false;
            //just send the player in a t-pose, aiming gun
        }

        GetComponentInChildren<EnemySpawner>()?.ClearSpawns();

        Vector3 shift = new Vector3(0, 0, transform.position.z * -2);
        transform.position = transform.position + shift;
        
    }

    public bool CanPlayerProgress()
    {
        if(enemySpawner == null) return true;

        if(enemySpawner.spawning) return false;

        return true;
    }

}
