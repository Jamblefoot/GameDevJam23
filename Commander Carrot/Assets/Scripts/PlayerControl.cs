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
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] Transform graphicsGimbal;
    [SerializeField] Transform graphicsRoot;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform gunpoint;
    float gunTilt = 60;

    Seat seat;
    Gun currentGun;

    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float jumpForce = 500f;
    [SerializeField] float stepSmooth = 0.1f;
    float headHeight = 1.5f;
    float waistHeight = 1f;
    float feetHeight = 0.1f;

    public MoveStyle moveStyle;
    //public AlignmentAxis alignmentAxis = AlignmentAxis.None;
    //public bool topDown;
    public Vector3 currentForward;
    public Vector3 sideNormal = Vector3.back;
    float shmupSpeed = 25f;

    Vector2 move;
    Vector2 look;
    Vector3 mousePos = Vector3.zero;
    [HideInInspector]
    public float horizontal, vertical, mouseScroll, lookHorizontal, lookVertical;
    Vector3 lookRot;
    bool jump, isGrounded;

    bool fire1Held = false;
    float fireDelay;

    [HideInInspector]
    public Rigidbody rigid;
    RigidbodyControl rigidControl;
    Transform tran;
    public Camera cam;
    FollowCamera followCam;

    public ShmupControl shmupControl;

    RaycastHit hit;

    public UnityEvent<Vector3> onHit;

    List<ParticleCollisionEvent> collisionEvents;


    
    void Start()
    {
        tran = transform;
        rigid = GetComponent<Rigidbody>();
        rigidControl = GetComponent<RigidbodyControl>();

        if(cameraPrefab != null)
        {
            foreach(Camera c in FindObjectsOfType<Camera>())
            {
                c.gameObject.SetActive(false);
            }

            cam = Instantiate(cameraPrefab, transform.position, Quaternion.identity).GetComponent<Camera>();
        }

        if(cam == null)
            cam = GetComponentInChildren<Camera>();
        else followCam = cam.GetComponent<FollowCamera>();
        followCam.MoveToTop(followCam.startAtTop, sideNormal);

        currentForward = tran.right;
    }

    void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
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
            fire1Held = true;
            if(currentGun != null)
                currentGun.Fire();
        }
        else fire1Held = false;
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
    void OnPause(InputValue value)
    {
        if(HudManager.singleton == null)
        {
            Debug.LogWarning("No hud manager, can't pause");
        }

        if(value.isPressed)
        {
            HudManager.singleton.PausMenuTogle();
        }
    }

    void Update()
    {
        horizontal = move.x;
        vertical = move.y;
        lookHorizontal = look.x;
        lookVertical = look.y;
        mousePos = Mouse.current.position.ReadValue();
        //Vector3 objectPos = cam.WorldToScreenPoint(tran.position);
        mousePos = cam.ScreenToWorldPoint(mousePos);
        switch(moveStyle)
        {
            case MoveStyle.Side:
                //constrain mousepos to constraintPlane
                //okay so we want the body to rotate 
                // and the arms to point towar d the position
                break;
            case MoveStyle.TopFree:
            case MoveStyle.TopShmup:
                mousePos.y = graphicsRoot.position.y;
                break;
        }

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
            RotateTowardsMouse(false);
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
        if (fire1Held && currentGun != null && currentGun.automatic)
        {
            fireDelay -= Time.deltaTime;
            if (fireDelay <= 0)
            {
                fireDelay = 0.1f;
                currentGun.Fire();
            }
        }

        if (moveStyle == MoveStyle.TopShmup)
        {
            RotateGunTowardsMouse();
        }
        else RotateTowardsMouse();

        Vector3 movement = Vector3.zero;
        if(Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0)
        {
            
            //float strafe = 0;

            switch(moveStyle)
            {
                case MoveStyle.Side:
                    movement = MoveSideScroll();
                    break;
                case MoveStyle.TopFree:
                    movement = currentForward * horizontal + Vector3.Cross(currentForward, tran.up) * vertical;
                    break;
                case MoveStyle.TopShmup:
                    movement = Vector3.forward * vertical;
                    MoveTopShmup();
                    break;
            }

            if(Vector3.Dot(movement, rigid.velocity) > 0)
            {
                if(rigid.velocity.sqrMagnitude > (maxSpeed * maxSpeed))
                    return;
            }

            movement = Vector3.ProjectOnPlane(movement, tran.up);

            if(StepCheck(movement))
                rigid.AddForce(movement, ForceMode.VelocityChange);
        }

        

        /*Vector3 aimDir = tran.forward;
        Vector3 gunDir = Vector3.zero;
            //Vector3 aimDir = FindNearestEnemy(movement);
            if(moveStyle == MoveStyle.Side)
            {
                aimDir = new Vector3(mousePos.x, graphicsRoot.position.y, mousePos.z) - graphicsRoot.position;
                gunDir = Vector3.ProjectOnPlane(mousePos, rigidControl.constraintPlane.normal);
            }
            else aimDir = mousePos - graphicsRoot.position;

            //graphicsGimbal.LookAt(transform.position + movement, Vector3.up);
            float angle = Vector3.SignedAngle(graphicsGimbal.forward, aimDir, tran.up);
            if(Mathf.Abs(angle) > 10f)
                graphicsGimbal.Rotate(tran.up, Mathf.Min(Mathf.Abs(angle), 10f) * Mathf.Sign(angle));
            else graphicsGimbal.LookAt(tran.position + aimDir, Vector3.up);

            if(gunDir != Vector3.zero)
                gunpoint.LookAt(gunDir, gunpoint.right);*/
        //}

        
    }

    void RotateTowardsMouse(bool useGimbal = true)
    {
        Vector3 aimDir = tran.forward;
        Vector3 gunDir = Vector3.zero;
        //Vector3 aimDir = FindNearestEnemy(movement);
        if (moveStyle == MoveStyle.Side)
        {
            aimDir = new Vector3(mousePos.x, graphicsRoot.position.y, mousePos.z) - graphicsRoot.position;
            //gunDir = Vector3.ProjectOnPlane(mousePos, rigidControl.constraintPlane.normal) + rigidControl.constraintPlane.GetDistanceToPoint(gunpoint.position) * rigidControl.constraintPlane.normal;
            gunDir = rigidControl.constraintPlane.ClosestPointOnPlane(mousePos) + rigidControl.constraintPlane.GetDistanceToPoint(gunpoint.position) * rigidControl.constraintPlane.normal;
        }
        else aimDir = mousePos - graphicsRoot.position;

        //graphicsGimbal.LookAt(transform.position + movement, Vector3.up);
        
        if(useGimbal)
        {
            float angle = Vector3.SignedAngle(graphicsGimbal.forward, aimDir, tran.up);
            if (Mathf.Abs(angle) > 10f)
                graphicsGimbal.Rotate(tran.up, Mathf.Min(Mathf.Abs(angle), 10f) * Mathf.Sign(angle));
            else graphicsGimbal.LookAt(tran.position + aimDir, Vector3.up);
        }
        else
        {
            float angle = Vector3.SignedAngle(graphicsRoot.forward, aimDir, tran.up);
            if (Mathf.Abs(angle) > 10f)
                graphicsRoot.Rotate(graphicsRoot.parent.up, Mathf.Min(Mathf.Abs(angle), 10f) * Mathf.Sign(angle));
            else graphicsRoot.LookAt(graphicsRoot.parent.position + aimDir, Vector3.up);
        }

        if (gunDir != Vector3.zero)
            gunpoint.LookAt(gunDir, graphicsRoot.right);
        else gunpoint.rotation = graphicsRoot.rotation * Quaternion.Euler(0,0,gunTilt);
    }

    void RotateGunTowardsMouse()
    {
        Vector3 aimDir = mousePos - graphicsRoot.position;

        float angle = Vector3.SignedAngle(gunpoint.forward, aimDir, tran.up);
        if (Mathf.Abs(angle) > 10f)
            gunpoint.Rotate(tran.up, Mathf.Min(Mathf.Abs(angle), 10f) * Mathf.Sign(angle));
        else gunpoint.LookAt(tran.position + aimDir, Vector3.up);
    }

    Vector3 FindNearestEnemy(Vector3 dir)
    {
        //TODO cast out cone in front looking for the nearest enemy
        //  if enemy found, return direction to enemy
        //  else return the original direction passed in

        return dir;
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

    void MoveTopShmup()
    {
        transform.position = transform.position + (horizontal * shmupControl.transform.right + vertical * transform.forward) * Time.deltaTime * shmupSpeed;
        Vector3 shmupLocal = shmupControl.transform.InverseTransformPoint(transform.position);
        if (Mathf.Abs(shmupLocal.x) > shmupControl.width)
            shmupLocal = new Vector3(Mathf.Sign(shmupLocal.x) * shmupControl.width, shmupLocal.y, shmupLocal.z);
        if (Mathf.Abs(shmupLocal.z) > shmupControl.length)
            shmupLocal = new Vector3(shmupLocal.x, shmupLocal.y, Mathf.Sign(shmupLocal.z) * shmupControl.length);
        transform.position = shmupControl.transform.TransformPoint(shmupLocal);

        if (horizontal > 0) transform.rotation = shmupControl.transform.rotation * Quaternion.Euler(0, 0, -30);
        else if (horizontal < 0) transform.rotation = shmupControl.transform.rotation * Quaternion.Euler(0, 0, 30);
        else transform.rotation = shmupControl.transform.rotation;

        if (shmupLocal.z > shmupControl.length * 0.8f && shmupControl.CanPlayerProgress())
        {
            shmupControl.MoveToGround(transform);
        }
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
        if(currentGun != null)
        {
            currentGun.transform.parent = null;
            currentGun.gameObject.AddComponent<Rigidbody>().AddForce(Vector3.up - graphicsRoot.forward * 2);
            Destroy(currentGun.gameObject, 5);
        }
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
        HudManager.singleton.UpdatePlayerHealth(health);

        ParticleSystem part = other.GetComponent<ParticleSystem>();
        part.GetCollisionEvents(other, collisionEvents);
        for(int i = 0; i < collisionEvents.Count; i++)
        {
            onHit.Invoke(collisionEvents[i].intersection);
        }
    }

}
