using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType{ Token, Gun };//Ammo
public class Pickup : MonoBehaviour
{
    [SerializeField] PickupType type;

    void OnTriggerEnter(Collider col)
    {
        PlayerControl pc = col.GetComponent<PlayerControl>();
        if(pc != null)
        {
            pc.TakePickup(type, transform.childCount <= 0 ? null : transform.GetChild(0));

            //TODO POOL THIS!
            Destroy(gameObject);//, transform.childCount <= 0 ? 0 : 0.1f);

            GetComponent<Collider>().enabled = false; 
        }
    }
}
