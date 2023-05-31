using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public int health = 3;

    float viewDistance = 30;
    Vector3 targetOffset = Vector3.up;

    [SerializeField] LayerMask playerLayers;
    [SerializeField] Transform gunpoint;
    Gun gun;

    Transform tran;
    Rigidbody rigid;
    RigidbodyControl rigidControl;
    Animator animator;

    //GameObject indicator;

    bool targetInView;

    
    // Start is called before the first frame update
    void Start()
    {
        tran = transform;
        rigid = GetComponent<Rigidbody>();
        rigidControl = GetComponent<RigidbodyControl>();

        //freeze ragdoll rigidbodies
        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if(rb != rigid)
                rb.isKinematic = true;
        }

        animator = GetComponentInChildren<Animator>();

        gun = Instantiate(PrefabControl.singleton.GetRandomGun(), gunpoint.position, gunpoint.rotation, gunpoint).GetComponent<Gun>();
        ArmIK armIK = GetComponentInChildren<ArmIK>();
        if (armIK != null)
            armIK.SetHandAnchors(gun.leftHandAnchor, gun.rightHandAnchor);

        
        /*Transform indicatorCanvas = GameObject.FindWithTag("IndicatorCanvas").transform;
        if(indicatorCanvas != null)
        {
            indicator = Instantiate(PrefabControl.singleton.enemyIndicator, indicatorCanvas.position, indicatorCanvas.rotation, indicatorCanvas);
            indicator.GetComponent<UIIndicator>().target = transform;
        }*/

        StartCoroutine(Patrol());
        StartCoroutine(LookForPlayer());
    }

    IEnumerator Patrol()
    {
        float wait = 0f;
        Vector3 lookPos;
        while(health > 0)
        {
            wait -= Time.deltaTime;
            if(rigidControl.enabled)
            {//PATROL INSIDE BUILDING. DON'T FALL OFF PLATFORMS
                if(!targetInView)
                {
                    if(wait < 0)
                    {
                        wait = Random.Range(3f, 10f);
                        lookPos = transform.position + new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));
                        RotateToLookAt(lookPos);
                    }
                }
                yield return new WaitForFixedUpdate();
            }
            else
            {//PATROL TOPDOWN
                if(!targetInView)
                {
                    if(wait < 0)
                    {
                        wait = Random.Range(3f, 10f);
                        lookPos = transform.position + new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));
                        RotateToLookAt(lookPos);
                        if(Random.value > 0.9f) 
                        {
                            yield return new WaitForSeconds(wait);
                            wait = 0;
                        }
                    }
                    else
                    {
                        rigid.AddForce(transform.forward);
                    }


                }

                yield return new WaitForFixedUpdate();
            }
        }
    }

    IEnumerator LookForPlayer()
    {
        PlayerControl player = FindObjectOfType<PlayerControl>();
        Vector3 playerPos;
        RaycastHit hit;
        if (player != null)
        {
            while (health > 0)
            {
                playerPos = player.GetWorldPosition() + targetOffset;
                if (Vector3.Distance(transform.position, playerPos) > viewDistance)
                {
                    targetInView = false;
                    yield return new WaitForSeconds(Random.Range(1f, 2f));
                }
                else if(Vector3.Dot(playerPos - tran.position, transform.forward) > 0.2f)
                {
                    targetInView = true;
                    RotateToLookAt(playerPos);
                    gunpoint.LookAt(playerPos, Vector3.up);
                    //look at player
                    //raycast if can shoot player
                    if (Physics.Raycast(tran.position, tran.forward, out hit, viewDistance, playerLayers, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.transform.root.GetComponent<PlayerControl>() || hit.transform.root.GetComponent<ShipDrive>())
                        {
                            Fire();
                            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                        }
                        else yield return new WaitForSeconds(Random.Range(1f, 2f));
                    }
                    else yield return new WaitForSeconds(Random.Range(1f, 2f));
                }
                else 
                {
                    targetInView = false;
                    yield return new WaitForSeconds(Random.Range(1f, 2f));
                }
            }


        }
    }

    void RotateToLookAt(Vector3 worldPos)
    {
        float angle = Vector3.SignedAngle(tran.forward, Vector3.ProjectOnPlane(worldPos - tran.position, Vector3.up), Vector3.up);
        tran.Rotate(new Vector3(0, angle, 0));
    }

    void Fire()
    {
        if(gun != null)
            gun.Fire();
    }

    void OnParticleCollision(GameObject other) 
    {
        //Debug.Log("ENEMY HIT BY A PARTICLE FROM " + other.transform.root.gameObject.name);
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        ParticleSystem part = other.GetComponent<ParticleSystem>();
        part.GetCollisionEvents(gameObject, collisionEvents);
        health -= collisionEvents.Count;
        if(health <= 0)
        {
            Die();
            
            for (int i = 0; i < collisionEvents.Count; i++)
            {
                Vector3 pos = collisionEvents[i].intersection;
                Vector3 force = collisionEvents[i].velocity * 100;
                foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                {
                    rb.AddExplosionForce(500, pos, 0.5f, 0);
                }
                //rigid.AddForce(force);
            }
        }
        else
        {
            RotateToLookAt(other.transform.position);
        }
    }

    public void Die()
    {
        rigid.constraints = RigidbodyConstraints.None;
        RigidbodyControl rbc = GetComponent<RigidbodyControl>();
        if(rbc) 
            rbc.enabled = false;

        //TURN ON RAGDOLL
        GetComponent<Collider>().enabled = false;
        animator.enabled = false;
        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if(rb != rigid)
                rb.isKinematic = false;
        }

        UIIndicatorControl uiic = GetComponent<UIIndicatorControl>();
        if(uiic != null)
            Destroy(uiic);
        //if (indicator != null)
        //    Destroy(indicator);
    }
    public void Die(float power, Vector3 explosionPos, float radius, float upwardMod)
    {
        rigid.constraints = RigidbodyConstraints.None;
        RigidbodyControl rbc = GetComponent<RigidbodyControl>();
        if (rbc)
            rbc.enabled = false;

        //TURN ON RAGDOLL
        GetComponent<Collider>().enabled = false;
        animator.enabled = false;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb != rigid)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(power, explosionPos, radius, upwardMod);
            }

        }

        //if (indicator != null)
        //    Destroy(indicator);
        UIIndicatorControl uiic = GetComponent<UIIndicatorControl>();
        if (uiic != null)
            Destroy(uiic);
    }
}
