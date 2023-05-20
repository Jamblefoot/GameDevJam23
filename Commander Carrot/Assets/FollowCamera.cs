using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 lookAtOffset = new Vector3(0, 1.5f, 0);

    public Vector3 sidePos = new Vector3(0, 1, -10);
    public Vector3 topPos = new Vector3(0, 10, -1);

    bool atTop;
    bool moving;

    Vector3 targetPos;

    void Start()
    {
        targetPos = sidePos;
    }
    void FixedUpdate()
    {
        if(target == null) return;

        transform.LookAt(target.position + lookAtOffset, Vector3.up);

        Vector3 currentPos = target.InverseTransformPoint(transform.position);
        if(currentPos != targetPos)
            transform.position = target.TransformPoint(Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 10f));
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

        Vector3 movePos = toTop ? topPos : sidePos;
        while(targetPos != movePos)
        {
            targetPos = Vector3.Lerp(targetPos, movePos, Time.deltaTime);
            yield return null;
        }

        moving = false;
    }
}
