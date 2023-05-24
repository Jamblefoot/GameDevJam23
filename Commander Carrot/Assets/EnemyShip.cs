using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    public ShmupControl shmupControl;
    public float horizontal;
    public float vertical;

    float speed = 10f;
    Rigidbody rigid;

    float health = 3;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(shmupControl != null)
        {
            transform.position = transform.position + (horizontal * transform.right + vertical * transform.forward) * Time.deltaTime * speed;
            Vector3 shmupLocal = shmupControl.transform.InverseTransformPoint(transform.position);
            if (Mathf.Abs(shmupLocal.x) > shmupControl.width)
            {
                horizontal *= -1;
                shmupLocal = new Vector3(Mathf.Sign(shmupLocal.x) * shmupControl.width, shmupLocal.y, shmupLocal.z);
            }
            //if (Mathf.Abs(shmupLocal.z) > shmupControl.length)
            //    shmupLocal = new Vector3(shmupLocal.x, shmupLocal.y, Mathf.Sign(shmupLocal.z) * shmupControl.length);
            transform.position = shmupControl.transform.TransformPoint(shmupLocal);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("ENEMY HIT BY A PARTICLE FROM " + other.transform.root.gameObject.name);
        health--;
        if (health <= 0)
        {
            Die();
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            ParticleSystem part = other.GetComponent<ParticleSystem>();
            part.GetCollisionEvents(other, collisionEvents);
            for (int i = 0; i < collisionEvents.Count; i++)
            {
                Vector3 pos = collisionEvents[i].intersection;
                Vector3 force = collisionEvents[i].velocity * 100;
                rigid.AddForce(force);
            }
        }
    }

    void Die()
    {
        rigid.constraints = RigidbodyConstraints.None;
        RigidbodyControl rbc = GetComponent<RigidbodyControl>();
        if (rbc)
            rbc.enabled = false;
        rigid.isKinematic = false;
        rigid.useGravity = true;


        //TODO ragdoll!
    }
}
