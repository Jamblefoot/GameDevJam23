using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpdateLoop{ Null, Fixed, Late, LateFixed};
public class FollowCamera : MonoBehaviour
{
    [SerializeField] UpdateLoop updateLoop = UpdateLoop.Null;

    public Transform target;
    public Vector3 lookAtOffset = new Vector3(0, 1.5f, 0);

    public Vector3 sidePos = new Vector3(0, 1, -10);
    public Vector3 topPos = new Vector3(0, 10, -1);
    float sideTilt = 0.01f;
    public float transitionSpeed = 10f;
    [SerializeField][Range(0.01f, 1f)]
    float smoothSpeed = 0.125f;

    public bool startAtTop;

    bool atTop, moving, fixedHappened;


    Vector3 targetPos;
    Vector3 lastPos = Vector3.zero;
    Vector3 targetVelocity = Vector3.zero;

    Transform tran;

    Vector3 velocity;

    public Vector3 sideNormal = Vector3.back;

    void Start()
    {
        targetPos = sidePos;
        if(target != null)
            lastPos = target.position;

        tran = transform;

        //MoveToTop(startAtTop, sideNormal);
    }
    void Update()
    {
        if(updateLoop == UpdateLoop.Null)
            UpdatePosition();
    }
    void FixedUpdate()
    {
        if(target != null)
        {
            targetVelocity = (target.position - lastPos)/Time.deltaTime;
            lastPos = target.position;
        }
        else targetVelocity = Vector3.zero;

        if(updateLoop == UpdateLoop.Fixed)
            UpdatePosition();
        fixedHappened = true;
    }
    void LateUpdate()
    {
        if(updateLoop == UpdateLoop.Late)
            UpdatePosition();
        else if(fixedHappened && updateLoop == UpdateLoop.LateFixed)
            UpdatePosition();
        fixedHappened = false;
    }

    void UpdatePosition()
    {
        if (target == null) return;

        if (atTop)
        {
            //tran.LookAt(target.position + lookAtOffset, Vector3.forward);
            tran.LookAt(tran.position + Vector3.down, Vector3.forward);
        }
        else 
        {
            //tran.LookAt(target.position + lookAtOffset, Vector3.up);
            tran.LookAt(tran.position - sideNormal + Vector3.down * sideTilt, Vector3.up);
        }

        Vector3 currentPos = target.root.InverseTransformPoint(tran.position);
        Vector3 projectedVelocity = atTop ? targetVelocity : Vector3.ProjectOnPlane(targetVelocity, Vector3.up);
        if (currentPos != targetPos)
            tran.position = Vector3.SmoothDamp(tran.position, target.root.TransformPoint(targetPos) + projectedVelocity * 0.3f, ref velocity, smoothSpeed);
            //tran.position = target.root.TransformPoint(targetPos);
        //    transform.position = target.TransformPoint(Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 10f));
    }

    public void MoveToTop(bool toTop, Vector3 sideNorm)//AlignmentAxis alignAxis)
    {
        sideNormal = sideNorm;
        if(moving)
            StopAllCoroutines();
        StartCoroutine(MoveCo(toTop));//, alignAxis));
    }
    IEnumerator MoveCo(bool toTop)//, AlignmentAxis alignAxis)
    {
        moving = true;

        atTop = toTop;

        //Vector3 movePos = toTop ? topPos : GetAlignmentRotation(alignAxis) * sidePos;
        Vector3 movePos = toTop ? topPos : GetAlignmentRotation(sideNormal) * sidePos;
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

    Quaternion GetAlignmentRotation(Vector3 sideNorm)
    {
        return Quaternion.Euler(0f, Vector3.SignedAngle(Vector3.back, sideNorm,Vector3.up), 0f);
    }
}
