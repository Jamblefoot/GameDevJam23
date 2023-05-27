using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDrive : MonoBehaviour
{
    [SerializeField] float enginePower = 500f;
    [SerializeField] float turnPower = 500f;
    [SerializeField] 
    float shmupSpeed = 25f;

    [SerializeField] ParticleSystem[] lasers;
    float fireDelay = 0.2f;
    [SerializeField] ParticleSystem exhaust;

    [HideInInspector]
    public Rigidbody rigid;

    float horizontal, vertical;
    bool driving, isDriving, fire, isFiring;

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
        exhaust.Play();
        while(driving)
        {
            if(fire && !isFiring)
                FireLasers();

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

                if (shmupLocal.z > shmupControl.length * 0.8f && shmupControl.CanPlayerProgress())
                {
                    shmupControl.MoveToGround(transform);
                }
            }

            yield return new WaitForFixedUpdate();
        }

        exhaust.Stop();

        fire = false;
        horizontal = 0;
        vertical = 0;
        isDriving = false;
    }

    public void ApplyInput(float hor, float vert, bool trigger)
    {
        horizontal = hor;
        vertical = vert;
        fire = trigger;
    }

    void FireLasers()
    {
        if(lasers.Length <= 0)
            return;

        if(!isFiring)
        {
            StartCoroutine(FireLasersCo());
        }
    }
    IEnumerator FireLasersCo()
    {
        isFiring = true;
        while(fire)
        {
            foreach(ParticleSystem l in lasers)
            {
                l.Emit(1);
            }
            yield return new WaitForSeconds(fireDelay);
        }
        isFiring = false;
    }
}
