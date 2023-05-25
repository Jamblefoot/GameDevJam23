using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDrive : MonoBehaviour
{
    [SerializeField] float enginePower = 500f;
    [SerializeField] float turnPower = 500f;
    [SerializeField] 
    float shmupSpeed = 25f;

    [HideInInspector]
    public Rigidbody rigid;

    float horizontal, vertical;
    bool driving;
    bool isDriving;

    public ShmupControl shmupControl;
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
            if(shmupControl == null)
            {
                rigid.AddTorque(transform.up * horizontal * Mathf.Sign(vertical) * turnPower, ForceMode.Force);
                rigid.AddForce(transform.forward * vertical * enginePower, ForceMode.Force);
            }
            else
            {
                transform.position = transform.position + (horizontal * shmupControl.transform.right + vertical * transform.forward) * Time.deltaTime * shmupSpeed;
                Vector3 shmupLocal = shmupControl.transform.InverseTransformPoint(transform.position);
                if(Mathf.Abs(shmupLocal.x) > shmupControl.width)
                    shmupLocal = new Vector3(Mathf.Sign(shmupLocal.x) * shmupControl.width, shmupLocal.y, shmupLocal.z);
                if(Mathf.Abs(shmupLocal.z) > shmupControl.length)
                    shmupLocal = new Vector3(shmupLocal.x, shmupLocal.y, Mathf.Sign(shmupLocal.z) * shmupControl.length);
                transform.position = shmupControl.transform.TransformPoint(shmupLocal);

                if(horizontal > 0) transform.rotation = shmupControl.transform.rotation * Quaternion.Euler(0, 0, -30);
                else if(horizontal < 0) transform.rotation = shmupControl.transform.rotation * Quaternion.Euler(0, 0, 30);
                else transform.rotation = shmupControl.transform.rotation;
                //TODO if horizontal > 0 tip to the right
                // if horizontal < 0 tip to left
            }

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