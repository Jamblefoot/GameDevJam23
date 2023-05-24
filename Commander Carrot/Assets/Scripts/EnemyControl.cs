using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public int health = 3;

    Transform tran;
    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        tran = transform;
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject other) 
    {
        Debug.Log("ENEMY HIT BY A PARTICLE FROM " + other.transform.root.gameObject.name);
        health--;
        if(health <= 0)
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
        if(rbc) 
            rbc.enabled = false;


        //TODO ragdoll!
    }
}
