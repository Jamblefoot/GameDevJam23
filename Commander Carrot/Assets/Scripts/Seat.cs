using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public Transform occupant;
    PlayerControl occupantPlayer;
    bool blockSeat;
    Collider blockCol;

    void OnTriggerEnter(Collider col)
    {
        if(occupant != null || blockSeat) return;

        PlayerControl pc = col.GetComponent<PlayerControl>();
        if(pc != null)
        {
            occupant = col.transform;
            occupantPlayer = pc;

            pc.TakeSeat(this);

            blockCol = col;
            blockSeat = true;
        }

    }

    void OnTriggerExit(Collider col)
    {
        if(col == blockCol)
            Invoke("ClearBlock", 0.5f);
    }
    void ClearBlock()
    {
        blockSeat = false;
        blockCol = null;
    }

    public void ClearOccupant()
    {
        occupant = null;
        occupantPlayer = null;
    }
}
