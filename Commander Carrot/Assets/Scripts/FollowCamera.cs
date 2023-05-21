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
            MoveToTop(true, AlignmentAxis.None);
        else MoveToTop(false, AlignmentAxis.None);
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

    public void MoveToTop(bool toTop, AlignmentAxis alignAxis)
    {
        if(moving)
            StopAllCoroutines();
        StartCoroutine(MoveCo(toTop, alignAxis));
    }
    IEnumerator MoveCo(bool toTop, AlignmentAxis alignAxis)
    {
        moving = true;

        atTop = toTop;

        Vector3 movePos = toTop ? topPos : GetAlignmentRotation(alignAxis) * sidePos;
        while(targetPos != movePos)
        {
            targetPos = Vector3.Lerp(targetPos, movePos, transitionSpeed * Time.deltaTime);
            yield return null;
        }

        moving = false;
    }

    Quaternion GetAlignmentRotation(AlignmentAxis axis)
    {
        switch(axis)
        {
            case AlignmentAxis.Z:
                return Quaternion.identity;
            case AlignmentAxis.X:
                return Quaternion.Euler(0, -90, 0);
            case AlignmentAxis.Y:
                return Quaternion.Euler(90, 0, 0);
        }

        return Quaternion.identity;
    }
}
