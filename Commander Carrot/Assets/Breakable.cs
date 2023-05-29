using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] GameObject brokenPrefab;
    int health = 3;
    void OnParticleCollision(GameObject other)
    {
        Debug.Log("ENEMY HIT BY A PARTICLE FROM " + other.transform.root.gameObject.name);
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        ParticleSystem part = other.GetComponent<ParticleSystem>();
        part.GetCollisionEvents(gameObject, collisionEvents);
        health -= collisionEvents.Count;
        if (health <= 0)
        {
            Break();
        }
    }

    void Break()
    {
        Destroy(Instantiate(brokenPrefab, transform.position, transform.rotation, transform.parent), 10f);
        Destroy(gameObject);
    }
}
