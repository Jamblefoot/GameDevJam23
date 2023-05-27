using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    float radius = 5f;
    float power = 500f;
    float delay = 3f;

    float timer;

    void Start()
    {
        timer = delay;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        Explode();
    }

    public void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach(Collider hit in colliders)
        {
            EnemyControl ec = hit.GetComponent<EnemyControl>();
            if(ec != null)
                ec.Die(power, explosionPos, radius, 3f);
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 3f);
        }

        Instantiate(PrefabControl.singleton.explosion, transform.position, transform.rotation);


        Destroy(gameObject);
    }
}
