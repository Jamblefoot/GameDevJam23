using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIndicator : MonoBehaviour
{
    public Transform target;
    RectTransform rtran;
    RectTransform parent;

    Vector3 screenPos;

    Image rend;
    void Start()
    {
        rtran = transform as RectTransform;
        parent = transform.parent as RectTransform;
        rend = GetComponent<Image>();
    }
    void Update()
    {
        if(target == null) return;

        screenPos = Camera.main.WorldToScreenPoint(target.position);
        if(screenPos.x > 0 && screenPos.x < parent.rect.width && screenPos.y > 0 && screenPos.y < parent.rect.height)
            rend.enabled = false;
        else rend.enabled = true;
        screenPos = new Vector3(Mathf.Clamp(screenPos.x, 0, parent.rect.width), Mathf.Clamp(screenPos.y, 0, parent.rect.height), screenPos.z);
        rtran.anchoredPosition = screenPos;
        
    }
}
