using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType {Pistol, Shotgun, M7}
public class PrefabControl : MonoBehaviour
{
    public static PrefabControl singleton;

    [Header("Pickups")]
    public GameObject coin;
    public GameObject healthPack;
    public GameObject[] guns;

    [Header("Effects")]
    public GameObject explosion;
    public GameObject bulletImpact;
    public GameObject smoke;

    void Awake()
    {
        if(PrefabControl.singleton != null && PrefabControl.singleton != this)
            Destroy(this);
        else PrefabControl.singleton = this;
    }

    public GameObject GetRandomGun()
    {
        if(guns == null || guns.Length <= 0) return null;

        return guns[Random.Range(0, guns.Length)];
    }
}
