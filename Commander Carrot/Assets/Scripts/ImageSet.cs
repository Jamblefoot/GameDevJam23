using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSet : MonoBehaviour
{
    public Sprite[] images;
    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetToImage(int index)
    {
        if(index < 0 || index >= images.Length)
            return;
        
        image.sprite = images[index];
    }
}
