using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDrive : MonoBehaviour
{
    [SerializeField] float enginePower = 500f;
    [SerializeField] float turnPower = 500f;

    Rigidbody rigid;

    float horizontal, vertical;
    bool driving;
    bool isDriving;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void Drive()
    {
        if(!isDriving)
        {
            driving = true;
            StartCoroutine(DriveCo());
        }
    }
    public void StopDriving()
    {
        driving = false;
    }

    IEnumerator DriveCo()
    {
        Debug.Log("SHIP SHOULD BE DRIVING!!!!!!");
        isDriving = true;
        while(driving)
        {
            rigid.AddTorque(transform.up * horizontal * Mathf.Sign(vertical) * turnPower, ForceMode.Force);
            rigid.AddForce(transform.forward * vertical * enginePower, ForceMode.Force);

            yield return new WaitForFixedUpdate();
        }

        isDriving = false;
    }

    public void ApplyInput(float hor, float vert)
    {
        horizontal = hor;
        vertical = vert;
    }
}
