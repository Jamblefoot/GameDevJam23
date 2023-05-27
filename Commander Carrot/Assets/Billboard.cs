using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform tran;
    // Start is called before the first frame update
    void Start()
    {
        tran = transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform, Camera.main.transform.up);
        transform.rotation = transform.rotation * Quaternion.Euler(90, 0, 0);
    }
}
