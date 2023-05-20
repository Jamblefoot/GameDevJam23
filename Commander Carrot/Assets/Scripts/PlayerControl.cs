using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MoveStyle { Side, TopFree, TopShmup }

public class PlayerControl : MonoBehaviour
{
    [SerializeField] LayerMask groundLayers;
    [SerializeField] Transform graphicsGimbal;

    float maxSpeed = 10f;
    float jumpForce = 500f;

    public MoveStyle moveStyle;
    //public bool topDown;
    public Vector3 currentForward;

    Vector2 move;
    [HideInInspector]
    public float horizontal, vertical;
    bool jump, isGrounded;

    Rigidbody rigid;
    Transform tran;
    public Camera cam;
    FollowCamera followCam;

    RaycastHit hit;
    
    void Start()
    {
        tran = transform;
        rigid = GetComponent<Rigidbody>();

        if(cam == null)
            cam = GetComponentInChildren<Camera>();
        else followCam = cam.GetComponent<FollowCamera>();

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
            followCam.MoveToTop(style > 0);

        }
    }

    void Update()
    {
        horizontal = move.x;
        vertical = move.y;
    }

    void FixedUpdate()
    {
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

            switch(moveStyle)
            {
                case MoveStyle.Side:
                    //move is sidescroller fashion
                    movement = Vector3.right * horizontal;//currentForward * horizontal;

                    break;
                case MoveStyle.TopFree:
                    //move in gta style - camera rotation locked down, player rotating
                    movement = currentForward * horizontal + Vector3.Cross(currentForward, tran.up) * vertical;
                    break;
                case MoveStyle.TopShmup:
                    //camera locked down to one forward trajectory, player aimed forward and strafing
                    movement = Vector3.forward;// * horizontal + Vector3.Cross(currentForward, tran.up) * vertical;

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

            rigid.AddForce(movement, ForceMode.VelocityChange);

            //TODO mantle/step up when moving into platform side

            

            //graphicsGimbal.LookAt(transform.position + movement, Vector3.up);
            float angle = Vector3.SignedAngle(graphicsGimbal.forward, movement, tran.up);
            if(Mathf.Abs(angle) > 10f)
                graphicsGimbal.Rotate(tran.up, Mathf.Min(Mathf.Abs(angle), 10f) * Mathf.Sign(angle));
            else graphicsGimbal.LookAt(tran.position + movement, Vector3.up);
        }
    }

    bool CheckGrounded()
    {
        if(Physics.Raycast(tran.position + tran.up * 0.1f, -tran.up, out hit, 0.2f, groundLayers, QueryTriggerInteraction.Ignore))
        {
            rigid.drag = 1f;
            return true;
        }

        rigid.drag = 0f;
        return false;
    }

    public void SetMoveStyle(MoveStyle newStyle)
    {
        moveStyle = newStyle;

        followCam.MoveToTop((int)newStyle > 0);
    }
    //TODO
    //  When player transitions from topdown free to topdown shmup, 
    // rotate the player and the map to a new north in the direction of travel
    // so when generating, we can alway stack square

}
