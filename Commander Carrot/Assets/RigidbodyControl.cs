using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyControl : MonoBehaviour
{
    public Plane constraintPlane = new Plane();
    Rigidbody rigid;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if(rigid.IsSleeping()) return;

        //Debug.Log("Adjusting Rigidbody Position");
        //rigid.MovePosition(constraintPlane.ClosestPointOnPlane(rigid.position));
        transform.position = constraintPlane.ClosestPointOnPlane(transform.position);
        rigid.velocity = Vector3.ProjectOnPlane(rigid.velocity, constraintPlane.normal);
    }
}
