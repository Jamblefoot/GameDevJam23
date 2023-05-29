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
    void Start()
    {
        rtran = transform as RectTransform;
        parent = transform.parent as RectTransform;
    }
    void Update()
    {
        if(target == null) return;

        screenPos = Camera.main.WorldToScreenPoint(target.position);
        screenPos = new Vector3(Mathf.Clamp(screenPos.x, 0, parent.rect.width), Mathf.Clamp(screenPos.y, 0, parent.rect.height), screenPos.z);
        rtran.anchoredPosition = screenPos;
        
    }
}
