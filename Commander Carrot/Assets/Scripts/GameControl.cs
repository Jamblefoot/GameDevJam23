using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public static GameControl singleton;

    public bool inMenu;

    void Awake()
    {
        if(GameControl.singleton != null && GameControl.singleton != this)
            DestroyImmediate(this);
        else GameControl.singleton = this;
    }
}
