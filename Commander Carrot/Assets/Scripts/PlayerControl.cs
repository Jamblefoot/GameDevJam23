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
    bool alive = true;
    public int score = 0;
    public int grenades = 0;
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] Transform graphicsGimbal;
    [SerializeField] Transform graphicsRoot;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform gunpoint;
    float gunTilt = 60;

    Seat seat;
    [HideInInspector] public float blockSeat;
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
    float throwForce = 500f;

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

    Animator anim;

    public ShmupControl shmupControl;

    RaycastHit hit;
    RaycastHit feethit;

    public UnityEvent<Vector3> onHit;

    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();


    
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
        {
            cam = GetComponentInChildren<Camera>();
            followCam = cam.GetComponent<FollowCamera>();
        }
        else followCam = cam.GetComponent<FollowCamera>();
        followCam.target = graphicsRoot;
        followCam.MoveToTop(followCam.startAtTop, sideNormal);

        //Debug.Log("CAMERA SHOULD HAVE MOVED TO CENTER ON PLAYER!!!!");

        currentForward = tran.right;

        anim = graphicsRoot.GetComponentInChildren<Animator>();

        HudManager.singleton.UpdateScoreText(score);

        Invoke("StartQuip", 1f);
    }
    void StartQuip()
    {
        if(TutorialControl.singleton != null)
            TutorialControl.singleton.SetTutorialText("It's a hell of a thing to wake up buried alive", 5f);
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
            if(currentGun != null && alive)
                currentGun.Fire();
        }
        else fire1Held = false;
    }
    void OnFire2(InputValue value)
    {
        if(grenades > 0 && alive)
        {
            ThrowGrenade();
            grenades--;
            HudManager.singleton.UpdateGrenadeCount(grenades);
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

        if(blockSeat > 0)
            blockSeat -= Time.deltaTime;

        
    }

    void SetCameraSize(float newSize)
    {
        if(cam == null) return;

        cam.orthographicSize = Mathf.Clamp(newSize, 0.1f, 15f);
    }

    void FixedUpdate()
    {
        if(health <= 0) return;

        if(graphicsRoot.parent != graphicsGimbal) //IS IN SEAT OR SOMTHING
        {
            graphicsRoot.root.GetComponent<ShipDrive>().ApplyInput(horizontal, vertical, fire1Held);
            RotateTowardsMouse(false);
            anim.SetFloat("Movement", 0);
            return;
        }


        isGrounded = CheckGrounded();

        if(isGrounded)
        {
            anim.SetFloat("Movement", Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical)));
        }
        else anim.SetFloat("Movement", 0);

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

        
    }

    void RotateTowardsMouse(bool useGimbal = true)
    {
        Vector3 aimDir = tran.forward;
        Vector3 gunDir = Vector3.zero;
        //Vector3 aimDir = FindNearestEnemy(movement);
        if (moveStyle == MoveStyle.Side)
        {
            Vector3 mousePlanePos = rigidControl.constraintPlane.ClosestPointOnPlane(mousePos);
            //aimDir = new Vector3(mousePos.x, graphicsRoot.position.y, mousePos.z) - graphicsRoot.position;
            aimDir = new Vector3(mousePlanePos.x, graphicsRoot.position.y, mousePlanePos.z) - graphicsRoot.position + rigidControl.constraintPlane.normal * (10f - Mathf.Clamp(Vector3.Distance(mousePlanePos, graphicsRoot.position), 0f, 10f));
            //gunDir = Vector3.ProjectOnPlane(mousePos, rigidControl.constraintPlane.normal) + rigidControl.constraintPlane.GetDistanceToPoint(gunpoint.position) * rigidControl.constraintPlane.normal;
            gunDir = mousePlanePos + rigidControl.constraintPlane.GetDistanceToPoint(gunpoint.position) * rigidControl.constraintPlane.normal;
        }
        else if(moveStyle == MoveStyle.TopShmup)
        {
            aimDir = new Plane(Vector3.up, graphicsRoot.parent.position).ClosestPointOnPlane(mousePos) - graphicsRoot.position;
        }
        else aimDir = mousePos - graphicsRoot.position;

        //graphicsGimbal.LookAt(transform.position + movement, Vector3.up);
        
        if(useGimbal)
        {
            float angle = Vector3.SignedAngle(graphicsGimbal.forward, aimDir, Vector3.up);//tran.up);
            if (Mathf.Abs(angle) > 10f)
                graphicsGimbal.Rotate(tran.up, Mathf.Min(Mathf.Abs(angle), 10f) * Mathf.Sign(angle));
            else graphicsGimbal.LookAt(tran.position + aimDir, Vector3.up);
        }
        else //is in vehicle
        {
            float angle = Vector3.SignedAngle(graphicsRoot.forward, aimDir, graphicsRoot.parent.up);
            if (Mathf.Abs(angle) > 10f)
                graphicsRoot.Rotate(graphicsRoot.parent.up, Mathf.Min(Mathf.Abs(angle), 10f) * Mathf.Sign(angle));
            else graphicsRoot.LookAt(graphicsRoot.parent.position + aimDir, graphicsRoot.parent.up);
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

    /*Vector3 MoveSideScroll(AlignmentAxis axis)
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
    }*/
    Vector3 MoveSideScroll()//Vector3 sideNorm)
    {
        return Vector3.Cross(sideNormal, Vector3.up).normalized * horizontal;
    }

    void MoveTopShmup()
    {
        tran.position = tran.position + (horizontal * shmupControl.transform.right + vertical * tran.forward) * Time.deltaTime * shmupSpeed;
        Vector3 shmupLocal = shmupControl.transform.InverseTransformPoint(tran.position);
        if (Mathf.Abs(shmupLocal.x) > shmupControl.width)
            shmupLocal = new Vector3(Mathf.Sign(shmupLocal.x) * shmupControl.width, shmupLocal.y, shmupLocal.z);
        if (Mathf.Abs(shmupLocal.z) > shmupControl.length)
            shmupLocal = new Vector3(shmupLocal.x, shmupLocal.y, Mathf.Sign(shmupLocal.z) * shmupControl.length);
        tran.position = shmupControl.transform.TransformPoint(shmupLocal);

        if (horizontal > 0) tran.rotation = shmupControl.transform.rotation * Quaternion.Euler(0, 0, -30);
        else if (horizontal < 0) tran.rotation = shmupControl.transform.rotation * Quaternion.Euler(0, 0, 30);
        else tran.rotation = shmupControl.transform.rotation;

        if (shmupLocal.z > shmupControl.length * 0.8f && shmupControl.CanPlayerProgress())
        {
            shmupControl.MoveToGround(tran);
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
        
        bool waistBlocked = Physics.SphereCast(tran.position + tran.up * waistHeight, 0.25f, move.normalized, out feethit, 0.6f - 0.25f, groundLayers, QueryTriggerInteraction.Ignore);
        bool feetBlocked = Physics.SphereCast(tran.position + tran.up * feetHeight, feetHeight - 0.01f, move.normalized, out feethit, 0.6f - (feetHeight - 0.01f), groundLayers, QueryTriggerInteraction.Ignore);

        //if(feetBlocked)
        //    Debug.Log("FEET HITTING THING!");
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

        blockSeat = 0.3f;
        tran.position = graphicsRoot.position;

        if(seat != null)
        {
            ShipDrive sd = seat.GetComponentInParent<ShipDrive>();
            if(sd != null && sd.shmupControl != null)
                shmupControl = sd.shmupControl;
            seat.enabled = false;
            seat.ClearOccupant();
            sd.rigid.isKinematic = false;
            sd.rigid.useGravity = true;
        }
        seat = null;
        graphicsRoot.parent = graphicsGimbal;
        graphicsRoot.localPosition = Vector3.zero;
        graphicsRoot.localRotation = Quaternion.identity;

        if(shmupControl != null)
        {
            rigid.isKinematic = true;
            rigid.useGravity = false;
            SetMoveStyle(MoveStyle.TopShmup, Vector3.up);
        }
    }

    public void TakePickup(PickupType pType, Transform item)
    {
        switch(pType)
        {
            case PickupType.Token:
                score++;
                HudManager.singleton.UpdateScoreText(score);
                break;
            case PickupType.Gun:
                if(item == null)
                    Debug.Log("Pickup should be gun but no gun attached to pickup?!");
                else TakeGun(item.GetComponent<Gun>());
                break;
            case PickupType.Health:
                health++;
                HudManager.singleton.UpdatePlayerHealth(health);
                break;
            case PickupType.Grenade:
                grenades++;
                HudManager.singleton.UpdateGrenadeCount(grenades);
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

        HudManager.singleton.UpdateGunImage(gun.type);

        followCam.lookAhead = true;

        ArmIK armIK = GetComponentInChildren<ArmIK>();
        if(armIK != null)
            armIK.SetHandAnchors(gun.leftHandAnchor, gun.rightHandAnchor);

        // TODO ADD SOUND - Gun cocking
    }

    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("PLAYER HIT BY A PARTICLE FROM " + other.transform.root.gameObject.name + "!!!!");
        health--;
        HudManager.singleton.UpdatePlayerHealth(health);

        if(alive && health <= 0)
        {
            alive = false;
            TutorialControl.singleton.SetTutorialText("YOU HAVE BECOME DEAD", 100);
        }

        ParticleSystem part = other.GetComponent<ParticleSystem>();
        part.GetCollisionEvents(gameObject, collisionEvents);
        for(int i = 0; i < collisionEvents.Count; i++)
        {
            onHit.Invoke(collisionEvents[i].intersection);
        }
    }

    void ThrowGrenade()
    {
        GameObject go = Instantiate(PrefabControl.singleton.grenade, gunpoint.position, gunpoint.rotation);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.velocity = rigid.velocity;
        rb.AddForce(gunpoint.forward * throwForce);
    }

    public Vector3 GetWorldPosition()
    {
        return graphicsRoot.position;
    }

}
