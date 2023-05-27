using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    float springForce = 20;

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb != null && !rb.isKinematic)
        {
            rb.AddForce(transform.up * springForce, ForceMode.Impulse);

            GetComponent<AudioSource>()?.Play();
        }
    }
}
