using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStyleTrigger : MonoBehaviour
{
    public MoveStyle moveStyle;
    
    void OnTriggerEnter(Collider col)
    {
        PlayerControl pc = col.GetComponent<PlayerControl>();
        if(pc != null)
        {
            pc.SetMoveStyle(moveStyle);
        }
    }
}
