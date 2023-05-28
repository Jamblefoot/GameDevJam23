using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Seat : MonoBehaviour
{
    public Transform occupant;
    PlayerControl occupantPlayer;
    bool blockSeat;
    Collider blockCol;

    public UnityEvent onOccupantEnter;
    public UnityEvent onOccupantLeave;

    void OnTriggerEnter(Collider col)
    {
        if(occupant != null || blockSeat) return;

        PlayerControl pc = col.GetComponent<PlayerControl>();
        if(pc != null && pc.blockSeat <= 0)
        {
            occupant = col.transform;
            occupantPlayer = pc;

            pc.TakeSeat(this);

            onOccupantEnter.Invoke();

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
        if(occupant != null)
            onOccupantLeave.Invoke();
        occupant = null;
        occupantPlayer = null;
    }

    public void RemoveOccupant()
    {
        if(occupantPlayer != null)
            occupantPlayer.LeaveSeat();
    }
}
