using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType{ Random, Token, Gun };//Ammo
public class Pickup : MonoBehaviour
{
    [SerializeField] PickupType type;

    void Start()
    {
        if(transform.childCount > 0)
        {
            if(GetComponentInChildren<Gun>())
                type = PickupType.Gun;
            else type = PickupType.Token;
            return;
        } 

        switch(type)
        {   
            case PickupType.Random:
                SetRandomContents();
                break;
            case PickupType.Token:
                type = PickupType.Token;
                Instantiate(PrefabControl.singleton.coin, transform.position, transform.rotation, transform);
                break;
            case PickupType.Gun:
                type = PickupType.Gun;
                Instantiate(PrefabControl.singleton.GetRandomGun(), transform.position, transform.rotation, transform);
                break;
        }

        Renderer rend = GetComponent<Renderer>();
        switch(type)
        {
            case PickupType.Token:
                rend.material.SetColor("_Color", new Color(1,1,0,0.5f));//yellow
                rend.material.SetColor("_EmissionColor", Color.yellow);
                break;
            case PickupType.Gun:
                rend.material.SetColor("_Color", new Color(0,0,1,0.5f));
                rend.material.SetColor("_EmissionColor", Color.blue);
                break;
        }
        
    }

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

    void SetRandomContents()
    {
        switch((PickupType)Random.Range(0, System.Enum.GetNames(typeof(PickupType)).Length))
        {
            case PickupType.Random:
                Destroy(gameObject);
                break;
            case PickupType.Token:
                type = PickupType.Token;
                Instantiate(PrefabControl.singleton.coin, transform.position, transform.rotation, transform);
                break;
            case PickupType.Gun:
                type = PickupType.Gun;
                Instantiate(PrefabControl.singleton.GetRandomGun(), transform.position, transform.rotation, transform);
                break;
        }
    }
}
