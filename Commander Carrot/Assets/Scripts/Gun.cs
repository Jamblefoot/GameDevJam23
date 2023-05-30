using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunType type;
    public int projectilesPerShot = 1;
    public float recoil;
    public Transform leftHandAnchor;
    public Transform rightHandAnchor;
    public ParticleSystem bulletParticles;
    public ParticleSystem muzzleParticles;
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip clickSound;
    public AudioClip reloadSound;// this'd be like a whole animation thing
    public AudioClip cockSound;
    //bulletPrefab?
    //cooldown
    public bool automatic = false;

    /*public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }*///THIS ISN'T WORKING!?
    public void Fire()
    {
        //if(ammo <= 0)
        //  TODO ADD AUDIO CLICK
        //else
        if(bulletParticles != null)
            bulletParticles.Emit(projectilesPerShot);
        if(muzzleParticles != null)
            muzzleParticles.Emit(25);
        Rigidbody rb = transform.root.GetComponent<Rigidbody>();
        if(rb != null)
            rb.AddForce(-transform.forward * recoil, ForceMode.Impulse);
        if(audioSource != null && fireSound != null)
            audioSource.PlayOneShot(fireSound);
    }
}
