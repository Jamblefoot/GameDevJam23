using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 lookAtOffset = new Vector3(0, 1.5f, 0);

    public Vector3 sidePos = new Vector3(0, 1, -10);
    public Vector3 topPos = new Vector3(0, 10, -1);
    public float transitionSpeed = 10f;

    public bool startAtTop;

    bool atTop;
    bool moving;

    Vector3 targetPos;

    Transform tran;

    void Start()
    {
        targetPos = sidePos;

        tran = transform;

        if(startAtTop)
            MoveToTop(true);
    }
    void FixedUpdate()
    {
        if(target == null) return;

        if(atTop)
            tran.LookAt(target.position + lookAtOffset, Vector3.forward);
        else tran.LookAt(target.position + lookAtOffset, Vector3.up);

        Vector3 currentPos = target.InverseTransformPoint(tran.position);
        if(currentPos != targetPos)
            tran.position = target.TransformPoint(targetPos);
        //    transform.position = target.TransformPoint(Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 10f));
    }

    public void MoveToTop(bool toTop)
    {
        if(moving)
            StopAllCoroutines();
        StartCoroutine(MoveCo(toTop));
    }
    IEnumerator MoveCo(bool toTop)
    {
        moving = true;

        atTop = toTop;

        Vector3 movePos = toTop ? topPos : sidePos;
        while(targetPos != movePos)
        {
            targetPos = Vector3.Lerp(targetPos, movePos, transitionSpeed * Time.deltaTime);
            yield return null;
        }

        moving = false;
    }
}
