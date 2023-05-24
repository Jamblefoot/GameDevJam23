using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum MoveStyle { Side, TopFree, TopShmup }
public enum AlignmentAxis { None, X, Y, Z };//Xneg, Yneg, Zneg

public class PlayerControl : MonoBehaviour
{
    public int health = 100;
    
    [SerializeField] LayerMask groundLayers;
    [SerializeField] Transform graphicsGimbal;
    [SerializeField] Transform graphicsRoot;
    [SerializeField] Transform gunpoint;

    Seat seat;
    Gun currentGun;

    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float jumpForce = 500f;
    [SerializeField] float stepSmooth = 0.1f;
    float headHeight = 1.5f;
    float waistHeight = 1f;
    float feetHeight = 0.1f;

    public MoveStyle moveStyle;
    public AlignmentAxis alignmentAxis = AlignmentAxis.None;
    //public bool topDown;
    public Vector3 currentForward;
    public Vector3 sideNormal = Vector3.back;

    Vector2 move;
    [HideInInspector]
    public float horizontal, vertical, mouseScroll;
    bool jump, isGrounded;

    Rigidbody rigid;
    Transform tran;
    public Camera cam;
    FollowCamera followCam;

    RaycastHit hit;

    public UnityEvent<Vector3> onHit;

    List<ParticleCollisionEvent> collisionEvents;
    
    void Start()
    {
        tran = transform;
        rigid = GetComponent<Rigidbody>();

        if(cam == null)
            cam = GetComponentInChildren<Camera>();
        else followCam = cam.GetComponent<FollowCamera>();
        followCam.MoveToTop(followCam.startAtTop, sideNormal);

        currentForward = tran.right;
    }

    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if(value.isPressed)
            jump = true;
    }
    void OnFire1(InputValue value)
    {
        if(value.isPressed)
        {
            if(currentGun != null)
                currentGun.Fire();
        }
    }
    void OnFire3(InputValue value)
    {
        if(value.isPressed)
            LeaveSeat();
    }
    void OnCamera(InputValue value)
    {
        if(value.isPressed)
        {
            int style = (int)moveStyle;
            style %= System.Enum.GetNames(typeof(MoveStyle)).Length;
            moveStyle = (MoveStyle)style;

            if(style <= 0)
                currentForward = tran.forward;
            //else currentForward = tran.right;
            followCam.MoveToTop(style > 0, sideNormal);//alignmentAxis);

        }
    }
    void OnScroll(InputValue value)
    {
        mouseScroll = value.Get<Vector2>().y;
    }

    void Update()
    {
        horizontal = move.x;
        vertical = move.y;

        if (Mathf.Abs(mouseScroll) > 0 && cam != null)//&& !GameControl.singleton.inMenu
        {
            SetCameraSize(cam.orthographicSize - mouseScroll);

        }
    }

    void SetCameraSize(float newSize)
    {
        if(cam == null) return;

        cam.orthographicSize = Mathf.Clamp(newSize, 0.1f, 15f);
    }

    void FixedUpdate()
    {
        if(graphicsRoot.parent != graphicsGimbal) //IS IN SEAT OR SOMTHING
        {
            graphicsRoot.root.GetComponent<ShipDrive>().ApplyInput(horizontal, vertical);
            return;
        }


        isGrounded = CheckGrounded();

        if(jump)
        {
            jump = false;
            if(isGrounded)
            {
                rigid.AddForce(tran.up * jumpForce);
            }
            //else //double jump?
        }

        if(Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0)
        {
            Vector3 movement = Vector3.zero;
            float strafe = 0;

            switch(moveStyle)
            {
                case MoveStyle.Side:
                    //move is sidescroller fashion
                    //movement = MoveSideScroll(alignmentAxis);
                    movement = MoveSideScroll();//sideNormal);
                    //movement = Vector3.right * horizontal;//currentForward * horizontal;

                    break;
                case MoveStyle.TopFree:
                    //move in gta style - camera rotation locked down, player rotating
                    movement = currentForward * horizontal + Vector3.Cross(currentForward, tran.up) * vertical;
                    break;
                case MoveStyle.TopShmup:
                    //camera locked down to one forward trajectory, player aimed forward and strafing
                    //player/vehicle locked aimed forward
                    //MoveStrafe(alignmentAxis)
                    movement = Vector3.forward * vertical;// * horizontal + Vector3.Cross(currentForward, tran.up) * vertical;
                    strafe = horizontal;
                    break;
            }
            /*if(!topDown)
            {
                movement = currentForward * horizontal;// + Vector3.Cross(currentForward, tran.up) * vertical;
            }
            else movement = currentForward * horizontal + Vector3.Cross(currentForward, tran.up) * vertical;
            //else movement = cam.transform.up * vertical + cam.transform.right * horizontal;*/

            if(Vector3.Dot(movement, rigid.velocity) > 0)
            {
                if(rigid.velocity.sqrMagnitude > (maxSpeed * maxSpeed))
                    return;
            }

            movement = Vector3.ProjectOnPlane(movement, tran.up);

            if(StepCheck(movement))
                rigid.AddForce(movement, ForceMode.VelocityChange);

            //TODO mantle/step up when moving into platform side

            

            //graphicsGimbal.LookAt(transform.position + movement, Vector3.up);
            float angle = Vector3.SignedAngle(graphicsGimbal.forward, movement, tran.up);
            if(Mathf.Abs(angle) > 10f)
                graphicsGimbal.Rotate(tran.up, Mathf.Min(Mathf.Abs(angle), 10f) * Mathf.Sign(angle));
            else graphicsGimbal.LookAt(tran.position + movement, Vector3.up);
        }
    }

    Vector3 MoveSideScroll(AlignmentAxis axis)
    {
        switch(axis)
        {
            case AlignmentAxis.X:
            case AlignmentAxis.Y: // shouldn't really be using this, this would be top-down
                return Vector3.forward * horizontal;
            case AlignmentAxis.None:
            case AlignmentAxis.Z:
                return Vector3.right * horizontal;
            
        }

        return Vector3.zero;
    }
    Vector3 MoveSideScroll()//Vector3 sideNorm)
    {
        return Vector3.Cross(sideNormal, Vector3.up).normalized * horizontal;
    }

    bool CheckGrounded()
    {
        if(Physics.SphereCast(tran.position + tran.up * (0.1f + 0.5f), 0.48f, -tran.up, out hit, 0.2f, groundLayers, QueryTriggerInteraction.Ignore))
        {
            rigid.drag = 1f;
            return true;
        }

        rigid.drag = 0f;
        return false;
    }
    bool StepCheck(Vector3 move)// returns whether player can move forward, hopefully so they either mantle or fall and not stick to wall
    {
        if(Physics.Raycast(tran.position + tran.up * headHeight, move.normalized, move.magnitude, groundLayers, QueryTriggerInteraction.Ignore))
            return false;
        
        bool waistBlocked = Physics.Raycast(tran.position + tran.up * waistHeight, move.normalized, 0.6f, groundLayers, QueryTriggerInteraction.Ignore);
        bool feetBlocked = Physics.Raycast(tran.position + tran.up * feetHeight, move.normalized, 0.6f, groundLayers, QueryTriggerInteraction.Ignore);

        if(feetBlocked)
            Debug.Log("FEET HITTING THING!");
        //if(waistBlocked)
        
        if(isGrounded && Vector3.Dot(hit.normal, tran.up) > 0.5f)
            return true;
        if(waistBlocked || feetBlocked)
        {
            rigid.MovePosition(rigid.position + tran.up * stepSmooth);
        }

        return true;

    }

    public void SetMoveStyle(MoveStyle newStyle, Vector3 sideNorm)//AlignmentAxis axis)
    {
        moveStyle = newStyle;
        //alignmentAxis = axis;
        sideNormal = sideNorm;
        followCam.MoveToTop((int)newStyle > 0, sideNorm);//axis);
    }
    //TODO
    //  When player transitions from topdown free to topdown shmup, 
    // rotate the player and the map to a new north in the direction of travel
    // so when generating, we can alway stack square




    public void TakeSeat(Seat newSeat)
    {
        if(graphicsRoot == null)
        {
            Debug.LogError("NO GRAPHICS ROOT ASSIGNED FOR REPARENTING TO SEAT");
            return;
        }

        if(seat != null && newSeat == null)
        {
            LeaveSeat();
            return;
        }

        graphicsRoot.parent = newSeat.transform;
        graphicsRoot.localPosition = Vector3.zero;
        graphicsRoot.localRotation = Quaternion.identity;

        seat = newSeat;

        tran.position = new Vector3(20000, 20000, 20000);

    }

    public void LeaveSeat()
    {
        if(graphicsRoot.parent == graphicsGimbal)
            return;

        tran.position = graphicsRoot.position;

        if(seat != null)
            seat.ClearOccupant();
        seat = null;
        graphicsRoot.parent = graphicsGimbal;
        graphicsRoot.localPosition = Vector3.zero;
        graphicsRoot.localRotation = Quaternion.identity;
    }

    public void TakePickup(PickupType pType, Transform item)
    {
        switch(pType)
        {
            case PickupType.Token:
                //get points or whatever
                //TODO ADD SOUND Chime
                break;
            case PickupType.Gun:
                //parent gun to gunpoint
                if(item == null)
                    Debug.Log("Pickup should be gun but no gun attached to pickup?!");
                else TakeGun(item.GetComponent<Gun>());
                //todo if already have gun, add ammo and don't parent
                break;
        }


    }

    public void TakeGun(Gun gun)
    {
        if(gun == null)
        {
            Debug.LogError("This ain't no gun!");
            return;
        }
        
        // TODO if currentGun != null, put away or throw away
        currentGun = gun;
        gun.transform.parent = gunpoint;
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;

        followCam.lookAhead = true;

        // TODO ADD SOUND - Gun cocking
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.LogError("PLAYER HIT BY A PARTICLE FROM " + other.transform.root.gameObject.name + "!!!!");
        health--;

        ParticleSystem part = other.GetComponent<ParticleSystem>();
        part.GetCollisionEvents(other, collisionEvents);
        for(int i = 0; i < collisionEvents.Count; i++)
        {
            onHit.Invoke(collisionEvents[i].intersection);
        }
    }

}
