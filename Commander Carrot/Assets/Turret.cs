using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] LayerMask playerLayers;
    [SerializeField] ParticleSystem[] lasers;

    AudioSource audioSource;

    Vector3 targetOffset = Vector3.up;

    float viewDistance = 30f;

    int health = 3;
    bool alive = true;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        StartCoroutine(LookForPlayer());
    }

    IEnumerator LookForPlayer()
    {
        PlayerControl player = FindObjectOfType<PlayerControl>();
        Vector3 playerPos;
        RaycastHit hit;
        if(player != null)
        {
            while(alive)
            {
                playerPos = player.GetWorldPosition() + targetOffset;
                if(Vector3.Distance(transform.position, playerPos) > viewDistance)
                {
                    yield return new WaitForSeconds(Random.Range(1f, 2f));
                }
                else 
                {
                    transform.LookAt(playerPos, transform.parent.up);
                    //look at player
                    //raycast if can shoot player
                    if(Physics.Raycast(transform.position, transform.forward, out hit, viewDistance, playerLayers, QueryTriggerInteraction.Ignore))
                    {
                        if(hit.transform.root.GetComponent<PlayerControl>() || hit.transform.root.GetComponent<ShipDrive>())
                        {
                            Fire();
                            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                        }
                        else yield return new WaitForSeconds(Random.Range(1f, 2f));
                    }
                    else yield return new WaitForSeconds(Random.Range(1f, 2f));
                }
            }


        }
    }

    void Fire()
    {
        foreach(ParticleSystem ps in lasers)
        {
            ps.Emit(1);
        }

        if(audioSource != null)
            audioSource.Play();
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("TURRET HIT BY A PARTICLE");
        health--;

        if(health <= 0)
        {
            alive = false;
            transform.parent = null;
            gameObject.AddComponent<Rigidbody>();
            Destroy(gameObject, 5f);
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
