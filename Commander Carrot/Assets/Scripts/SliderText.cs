using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderText : MonoBehaviour
{
    TMP_Text text;
    Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        slider = GetComponentInParent<Slider>();
        UpdateText();
    }

    public void UpdateText()
    {
        text.text = slider.value.ToString() + "/" + slider.maxValue;
    }
}
