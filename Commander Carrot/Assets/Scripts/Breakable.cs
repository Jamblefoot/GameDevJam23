using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] GameObject brokenPrefab;
    [SerializeField] GameObject spawnPrefab;
    [SerializeField][Range(0f,1f)]
    float spawnProbability = 0.7f;
    int maxSpawn = 3;
    float despawnTime = 30;

    int health = 3;
    void OnParticleCollision(GameObject other)
    {
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
        if(spawnPrefab != null && Random.value < spawnProbability)
        {
            int spawnCount = Random.Range(1, maxSpawn + 1);
            while(spawnCount > 0)
            {
                Destroy(
                    Instantiate(spawnPrefab, 
                        transform.position + new Vector3(Random.Range(-0.25f, 0.25f), 0, Random.Range(-0.25f, 0.25f)), 
                        Quaternion.identity),
                    despawnTime);
                spawnCount--;
            }
        }

        Destroy(Instantiate(brokenPrefab, transform.position, transform.rotation, transform.parent), 10f);
        Destroy(gameObject);
    }
}
