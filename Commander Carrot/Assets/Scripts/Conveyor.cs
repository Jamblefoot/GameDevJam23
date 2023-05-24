using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 2.0f;

    //public float scrollX = 0f;
    public float scrollY = 0.5f;

    public bool moveChildren = true;
    public List<Transform> moveExcludes = new List<Transform>();

    Rigidbody rigid;
    Renderer rend;
    Material mat;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;
    }

    void FixedUpdate()
    {
        if (rigid == null) return;

        Vector3 move = transform.forward * speed * Time.deltaTime;

        rigid.position -= transform.forward * speed * Time.deltaTime;
        rigid.MovePosition(rigid.position + move);

        /*if (rend != null)
        {
            //float offsetX = Time.time * scrollX * speed;
            float offsetX = 0f;
            float offsetY = Time.time * (scrollY / transform.localScale.z) * speed;
            mat.mainTextureOffset = new Vector2(offsetX, offsetY);
        }*/

        foreach (Transform child in transform)
        {
            if (!moveExcludes.Contains(child) && child.parent == transform)
            {
                child.position += move;
                Vector3 localPos = transform.InverseTransformPoint(child.position);
                if (Mathf.Abs(localPos.z) > 0.5f || Mathf.Abs(localPos.x) > 0.5f)
                    child.parent = null;
            }
        }

    }

    void Update()
    {
        if (rend != null)
        {
            //float offsetX = Time.time * scrollX * speed;
            float offsetX = 0f;
            float offsetY = Time.time * (scrollY / transform.localScale.z) * speed;
            mat.mainTextureOffset = new Vector2(offsetX, offsetY);
        }
    }
}