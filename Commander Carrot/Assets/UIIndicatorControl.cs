using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIndicatorControl : MonoBehaviour
{
    [SerializeField] GameObject indicatorPrefab;
    GameObject indicator;
    // Start is called before the first frame update
    void Start()
    {
        Transform indicatorCanvas = GameObject.FindWithTag("IndicatorCanvas").transform;
        if (indicatorCanvas != null)
        {
            indicator = Instantiate(indicatorPrefab, indicatorCanvas.position, indicatorCanvas.rotation, indicatorCanvas);
            indicator.GetComponent<UIIndicator>().target = transform;
        }
    }

    void OnDestroy()
    {
        Destroy(indicator);
    }
}
